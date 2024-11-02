using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DockerBackup.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddsScheduleAndVolumesEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContainerName = table.Column<string>(type: "TEXT", nullable: false),
                    FrequencyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Hour = table.Column<int>(type: "INTEGER", nullable: false),
                    Minute = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContainerVolumes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContainerPath = table.Column<string>(type: "TEXT", nullable: false),
                    HostPath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerVolumes_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerVolumes_ScheduleId",
                table: "ContainerVolumes",
                column: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContainerVolumes");

            migrationBuilder.DropTable(
                name: "Schedules");
        }
    }
}
