using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogIdentityApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSendEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SendEmail",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendEmail",
                table: "AspNetUsers");
        }
    }
}
