using DockerBackup.Model.Models;

namespace DockerBackup.Business.Interfaces.Services;

public interface IContainersService
{
    Task<List<DockerBackup.Model.Models.Container>> GetAllAsync();
    Task<List<ContainerVolume>> GetAllVolumesAsync(string containerName);
}