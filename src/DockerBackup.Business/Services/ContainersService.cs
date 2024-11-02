using System.ComponentModel;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerBackup.Business.Interfaces.Services;
using DockerBackup.Model.Models;
using Microsoft.Extensions.Logging;

namespace DockerBackup.Business.Services;

public class ContainersService : IContainersService
{
    private readonly ILogger<ContainersService> _logger;
    private readonly IDockerClient _client;

    public ContainersService(ILogger<ContainersService> logger, IDockerClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<List<DockerBackup.Model.Models.Container>> GetAllAsync()
    {
        var clients = await _client.Containers.ListContainersAsync(new ContainersListParameters(){All = true});

        return clients.Select(c => new DockerBackup.Model.Models.Container()
        {
            Id = c.ID,
            Command = c.Command,
            Image = c.Image,
            ImageId = c.ImageID,
            Name = c.Names.FirstOrDefault()
        }).ToList();
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