using DockerBackup.Business.Interfaces.Models;
using DockerBackup.Model.Enums;

namespace DockerBackup.Business.Models;

public class ContainerSchedule : IContainerSchedule
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string Image { get; set; }
    public string ImageId { get; set; }
    public string Command { get; set; }
    public bool IsBackedUp { get; set; }
    public FrequencyTypes? Frequency { get; set; }
    public int? Hour { get; set; }
    public int? Minute { get; set; }
}