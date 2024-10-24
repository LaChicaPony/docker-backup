using System.Security;

namespace DockerBackup.Business.Interfaces.Helpers;

public interface IPasswordGenerator
{
    string Generate();
}