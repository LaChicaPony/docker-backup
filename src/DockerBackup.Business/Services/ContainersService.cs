using System.ComponentModel;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerBackup.Business.Interfaces.Services;
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
            Name = c.Names.FirstOrDefault(),
            Mounts = c.Mounts.Select(m => m.Source).ToList()
        }).ToList();
    }
}