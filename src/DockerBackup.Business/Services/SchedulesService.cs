using Docker.DotNet.Models;
using DockerBackup.Business.Interfaces.Services;
using DockerBackup.Model;
using DockerBackup.Model.Enums;
using DockerBackup.Model.Models;
using Microsoft.Extensions.Logging;

namespace DockerBackup.Business.Services;

public class SchedulesService : ISchedulesService
{
    private readonly ILogger<SchedulesService> _logger;
    private readonly IContainersService _containersService;
    private readonly BackupsDbContext _db;

    public SchedulesService(ILogger<SchedulesService> logger, IContainersService containersService, BackupsDbContext db)
    {
        _logger = logger;
        _containersService = containersService;
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

        //TODO: Enable job at job scheduler when implemented
    }
}