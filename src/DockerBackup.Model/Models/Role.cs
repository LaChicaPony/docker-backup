using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DockerBackup.Model.Models;

[Table("Roles")]
public class Role : IdentityRole
{
    
}