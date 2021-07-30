using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Xz.Node.Repository.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Test_Om",
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
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_Om", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Test_On",
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
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_On", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Test_On_Om",
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
                    TestOnKey = table.Column<Guid>(nullable: false),
                    TestOmKey = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_On_Om", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Test_On_Om_Test_Om_TestOmKey",
                        column: x => x.TestOmKey,
                        principalTable: "Test_Om",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Test_On_Om_Test_On_TestOnKey",
                        column: x => x.TestOnKey,
                        principalTable: "Test_On",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Test_On_Om_TestOmKey",
                table: "Test_On_Om",
                column: "TestOmKey");

            migrationBuilder.CreateIndex(
                name: "IX_Test_On_Om_TestOnKey",
                table: "Test_On_Om",
                column: "TestOnKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Test_On_Om");

            migrationBuilder.DropTable(
                name: "Test_Om");

            migrationBuilder.DropTable(
                name: "Test_On");
        }
    }
}
