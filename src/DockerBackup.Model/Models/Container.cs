namespace DockerBackup.Model.Models;

public class Container
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string Image { get; set; }
    public string  ImageId { get; set; }
    public string Command { get; set; }
    public List<string> Mounts { get; set; }
}