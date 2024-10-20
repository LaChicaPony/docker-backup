using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DockerBackup.Model;

public class BackupsDbContext : IdentityDbContext
{
    public BackupsDbContext(DbContextOptions<BackupsDbContext> options): base(options)
    {
    }
}