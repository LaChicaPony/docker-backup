using Docker.DotNet.Models;
using DockerBackup.App.Jobs;
using DockerBackup.Business.Interfaces.Services;
using DockerBackup.Model;
using DockerBackup.Model.Enums;
using DockerBackup.Model.Models;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DockerBackup.Business.Services;

public class SchedulesService : ISchedulesService
{
    private readonly ILogger<SchedulesService> _logger;
    private readonly IContainersService _containersService;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly BackupsDbContext _db;

    public SchedulesService(ILogger<SchedulesService> logger, IContainersService containersService, ISchedulerFactory schedulerFactory, BackupsDbContext db)
    {
        _logger = logger;
        _containersService = containersService;
        _schedulerFactory = schedulerFactory;
        _db = db;
    }

    public async Task AddAsync(string container, FrequencyTypes frequency, int hour, int minute, List<string> hostPaths)
    {
        var allVolumes = await _containersService.GetAllVolumesAsync(container);

        var schedule = new Schedule()
        {
            ContainerName = container,
            FrequencyId = (int)frequency,
            Hour = hour,
            Minute = minute
        };
        _db.Schedules.Add(schedule);
        await _db.SaveChangesAsync();

        var volumes = allVolumes.Where(v => hostPaths.Contains(v.HostPath)).ToList();
        volumes.ForEach(v =>
        {
            v.ScheduleId = schedule.Id;
            v.Schedule = null;
        });
        _db.ContainerVolumes.AddRange(volumes);
        await _db.SaveChangesAsync();

        _logger.LogInformation($"Scheduling job for '{container}' container at {hour}:{minute} every day");

        var newJob = JobBuilder.Create<DockerVolumeBackupJob>()
                               .WithIdentity($"{container}_backup_job")
                               .UsingJobData("container", container)
                               .Build();
        var newTrigger = TriggerBuilder.Create()
                                        .WithIdentity($"{container}_backup_job_trigger")
                                        .StartNow()
                                        .WithCronSchedule($"0 {minute} {hour} ? * *")
                                        .Build();

        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.ScheduleJob(newJob, newTrigger);

        _logger.LogInformation($"Job scheduled successfully.");
    }

    public async Task DeleteAsync(string container)
    {
        //TODO: Disable job at job scheduler when implemented

        var schedules = _db.Schedules.Where(x => x.ContainerName == container).ToList();

        foreach (var schedule in schedules)
        {
            var volumes = _db.ContainerVolumes.Where(x => x.ScheduleId == schedule.Id).ToList();
            _db.ContainerVolumes.RemoveRange(volumes);

            await _db.SaveChangesAsync();
        }

        _db.Schedules.RemoveRange(schedules);
        await _db.SaveChangesAsync();

        _logger.LogInformation($"Deleting scheduled job for '{container}' container...");

        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.DeleteJob(JobKey.Create($"{container}_backup_job"));

        _logger.LogInformation($"scheduled job deleted successfully.");
    }
}