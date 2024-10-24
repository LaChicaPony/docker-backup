using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DockerBackup.Model.Models;

[Table("Users")]
public class User : IdentityUser
{
    
}