using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Xz.Node.Repository.Migrations
{
    public partial class AddTestCode1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Test_Code",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDelete = table.Column<bool>(nullable: false),
                    Creater = table.Column<string>(nullable: true),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Updater = table.Column<string>(nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: false),
                    Column1 = table.Column<string>(nullable: true),
                    Column2 = table.Column<string>(nullable: true),
                    Column3 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_Code", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Test_Code");
        }
    }
}
