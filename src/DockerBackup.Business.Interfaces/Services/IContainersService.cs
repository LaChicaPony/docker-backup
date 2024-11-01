namespace DockerBackup.Business.Interfaces.Services;

public interface IContainersService
{
    public Task<List<DockerBackup.Model.Models.Container>> GetAllAsync();
}