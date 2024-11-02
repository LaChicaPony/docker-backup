using DockerBackup.Model.Enums;
using DockerBackup.Model.Models;

namespace DockerBackup.App.Models;

public class SaveScheduleVM
{
    public FrequencyTypes Frequency { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public List<string> SelectedVolumes { get; set; }
    public string ContainerName { get; set; }

    public List<ContainerVolume> ContainerVolumes { get; set; }
}