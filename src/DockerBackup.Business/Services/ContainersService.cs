using System.ComponentModel;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerBackup.Business.Interfaces.Models;
using DockerBackup.Business.Interfaces.Services;
using DockerBackup.Business.Models;
using DockerBackup.Model;
using DockerBackup.Model.Enums;
using DockerBackup.Model.Models;
using Microsoft.Extensions.Logging;

namespace DockerBackup.Business.Services;

public class ContainersService : IContainersService
{
    private readonly ILogger<ContainersService> _logger;
    private readonly IDockerClient _client;
    private readonly BackupsDbContext _db;

    public ContainersService(ILogger<ContainersService> logger, IDockerClient client, BackupsDbContext db)
    {
        _logger = logger;
        _client = client;
        _db = db;
    }

    public async Task<List<IContainerSchedule>> GetAllAsync()
    {
        //Get Current Containers
        var containers = (await _client.Containers.ListContainersAsync(new ContainersListParameters(){All = true})).ToList();

        //Get scheduled backups
        var allSchedules = _db.Schedules.ToDictionary(x => x.ContainerName, x => x);

        var missingContainers = allSchedules.Where(s => !containers.Any(c => c.Names.Contains(s.Key))).ToList();

        var result = new List<IContainerSchedule>();
        result.AddRange(containers.Select(x =>
        {
            var schedule = allSchedules.ContainsKey(x.Names.FirstOrDefault()) ? allSchedules[x.Names.FirstOrDefault()] : null;

            return new ContainerSchedule()
            {
                Name = x.Names.FirstOrDefault(),
                Command = x.Command,
                Id = x.ID,
                Image = x.Image,
                ImageId = x.ImageID,
                IsBackedUp = schedule != null,
                Hour = schedule?.Hour,
                Minute = schedule?.Minute,
                Frequency = schedule != null ? (FrequencyTypes) schedule.FrequencyId : null
            };
        }));
        result.AddRange(missingContainers.Select(x => new ContainerSchedule()
        {
            Name = x.Key,
            Frequency = (FrequencyTypes) x.Value.FrequencyId,
            Hour = x.Value.Hour,
            Minute = x.Value.Minute,
            IsBackedUp = true
        }));

        return result;
    }

    public async Task<List<ContainerVolume>> GetAllVolumesAsync(string containerName)
    {
        var filters = new List<KeyValuePair<string, IDictionary<string, bool>>>()
        {
            new KeyValuePair<string, IDictionary<string, bool>>("name", new Dictionary<string, bool>(new List<KeyValuePair<string, bool>>()
            {
                new KeyValuePair<string, bool>(containerName, true)
            }))
        };
        var container = await _client.Containers.ListContainersAsync(new ContainersListParameters() { Filters = new Dictionary<string, IDictionary<string, bool>>(filters)});

        if (container == null || !container.Any())
        {
            return new List<ContainerVolume>();
        }

        return container[0].Mounts.Select(m => new ContainerVolume() { HostPath = m.Source, ContainerPath = m.Destination, Driver = m.Driver, Schedule = new Schedule(){ ContainerName = containerName }}).ToList();
    }
}