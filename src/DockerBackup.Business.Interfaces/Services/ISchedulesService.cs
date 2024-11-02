using DockerBackup.Model.Enums;

namespace DockerBackup.Business.Interfaces.Services;

public interface ISchedulesService
{
    Task AddAsync(string container, FrequencyTypes frequency, int hour, int minute, List<string> hostPaths);
}