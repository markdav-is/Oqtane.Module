using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RootNamespace.Migrations
{
    public partial class ModuleNameInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RootNamespaceModuleName",
                columns: table => new
                {
                    ModuleNameId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("Sqlite:Autoincrement", true)
                        .Annotation("MySql:ValueGenerationStrategy", 2),
                    ModuleId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootNamespaceModuleName", x => x.ModuleNameId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RootNamespaceModuleName");
        }
    }
}
