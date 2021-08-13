using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkieFileProcessor.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "filereferences",
                columns: table => new
                {
                    filehash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_filereferences", x => x.filehash);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "filereferences");
        }
    }
}
