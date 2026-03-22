using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateuserconfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "DomainUsers");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Username",
                table: "DomainUsers",
                newName: "IX_DomainUsers_Username");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DomainUsers",
                table: "DomainUsers",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DomainUsers",
                table: "DomainUsers");

            migrationBuilder.RenameTable(
                name: "DomainUsers",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_DomainUsers_Username",
                table: "Users",
                newName: "IX_Users_Username");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }
    }
}
