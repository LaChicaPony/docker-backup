using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DockerBackup.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddsDriverToContainerVolumesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Driver",
                table: "ContainerVolumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Driver",
                table: "ContainerVolumes");
        }
    }
}
