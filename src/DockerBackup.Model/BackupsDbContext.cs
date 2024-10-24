using DockerBackup.Model.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DockerBackup.Model;

public class BackupsDbContext : IdentityDbContext<User,Role, string>
{
    public BackupsDbContext(DbContextOptions<BackupsDbContext> options): base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(entity =>
        {
            entity.ToTable(name: "Users");
        });

        builder.Entity<Role>(entity =>
        {
            entity.ToTable(name: "Roles");
        });
        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
            //in case you chagned the TKey type
            //  entity.HasKey(key => new { key.UserId, key.RoleId });
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
            //in case you chagned the TKey type
            //  entity.HasKey(key => new { key.ProviderKey, key.LoginProvider });       
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");

        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
            //in case you chagned the TKey type
            // entity.HasKey(key => new { key.UserId, key.LoginProvider, key.Name });

        });
    }
}