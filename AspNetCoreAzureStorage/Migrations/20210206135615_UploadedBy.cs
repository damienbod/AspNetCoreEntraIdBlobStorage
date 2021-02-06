using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCoreAzureStorage.Migrations
{
    public partial class UploadedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "FileDescriptions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "FileDescriptions");
        }
    }
}
