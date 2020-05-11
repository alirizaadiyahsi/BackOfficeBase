using Microsoft.EntityFrameworkCore.Migrations;

namespace BackOfficeBase.DataAccess.Migrations
{
    public partial class Removed_Code_From_OU : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "OrganizationUnit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "OrganizationUnit",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
