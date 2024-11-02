using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DockerBackup.Model.Models;

public class Schedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string ContainerName { get; set; }

    public int FrequencyId { get; set; }

    public int Hour { get; set; }

    public int Minute { get; set; }
}