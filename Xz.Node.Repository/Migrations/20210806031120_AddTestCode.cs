using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Xz.Node.Repository.Migrations
{
    public partial class AddTestCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SysTables",
                columns: table => new
                {
                    TableName = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    TableDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysTables");
        }
    }
}
