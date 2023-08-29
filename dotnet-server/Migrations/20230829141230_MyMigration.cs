using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_server.Migrations
{
    /// <inheritdoc />
    public partial class MyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationName);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Desks",
                columns: table => new
                {
                    DeskName = table.Column<string>(type: "TEXT", nullable: false),
                    LocationName = table.Column<string>(type: "TEXT", nullable: false),
                    BookerUsername = table.Column<string>(type: "TEXT", nullable: false),
                    BookingStartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BookingEndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LocationName1 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desks", x => x.DeskName);
                    table.ForeignKey(
                        name: "FK_Desks_Locations_LocationName1",
                        column: x => x.LocationName1,
                        principalTable: "Locations",
                        principalColumn: "LocationName");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Desks_LocationName1",
                table: "Desks",
                column: "LocationName1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Desks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
