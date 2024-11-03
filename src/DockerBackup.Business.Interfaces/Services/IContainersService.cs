using DockerBackup.Business.Interfaces.Models;
using DockerBackup.Model.Models;

namespace DockerBackup.Business.Interfaces.Services;

public interface IContainersService
{
    Task<List<IContainerSchedule>> GetAllAsync();
    Task<List<ContainerVolume>> GetAllVolumesAsync(string containerName);
}