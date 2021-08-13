using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bulkie.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bulkies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    completed = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bulkies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bulkiefiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    bulkieid = table.Column<Guid>(type: "uuid", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    completed = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    filename = table.Column<string>(type: "text", nullable: true),
                    filereferenceid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bulkiefiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_bulkiefiles_bulkies_bulkieid",
                        column: x => x.bulkieid,
                        principalTable: "bulkies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bulkiefiles_bulkieid",
                table: "bulkiefiles",
                column: "bulkieid");

            migrationBuilder.CreateIndex(
                name: "ix_bulkiefiles_status",
                table: "bulkiefiles",
                column: "status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bulkiefiles");

            migrationBuilder.DropTable(
                name: "bulkies");
        }
    }
}
