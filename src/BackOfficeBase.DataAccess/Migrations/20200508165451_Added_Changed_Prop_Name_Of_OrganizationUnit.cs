using Microsoft.EntityFrameworkCore.Migrations;

namespace BackOfficeBase.DataAccess.Migrations
{
    public partial class Added_Changed_Prop_Name_Of_OrganizationUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "OrganizationUnit");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrganizationUnit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrganizationUnit");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "OrganizationUnit",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
