using DockerBackup.Model.Models;

namespace DockerBackup.App.Models;

public class ContainerVM
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string Image { get; set; }

    public ContainerVM(Container c)
    {
        Id = c.Id;
        Name = c.Name;
        Image = c.Image;
    }
}