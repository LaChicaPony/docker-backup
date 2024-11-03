using DockerBackup.Model.Enums;

namespace DockerBackup.Business.Interfaces.Models;

public interface IContainerSchedule
{
    string Id { get; set; }
    string? Name { get; set; }
    string Image { get; set; }
    string ImageId { get; set; }
    string Command { get; set; }
    bool IsBackedUp { get; set; }
    FrequencyTypes? Frequency { get; set; }
    int? Hour { get; set; }
    int? Minute { get; set; }
}