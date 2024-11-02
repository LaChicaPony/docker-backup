using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DockerBackup.Model.Models;

public class ContainerVolume
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(ScheduleId))]
    public int ScheduleId { get; set; }

    public string ContainerPath { get; set; }

    public string HostPath { get; set; }

    public string? Driver { get; set; }

    public Schedule Schedule { get; set; }

}