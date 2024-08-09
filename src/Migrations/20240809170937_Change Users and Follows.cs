using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogIdentityApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUsersandFollows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FollowerId",
                table: "Followers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowerId",
                table: "Followers",
                column: "FollowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerId",
                table: "Followers",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerId",
                table: "Followers");

            migrationBuilder.DropIndex(
                name: "IX_Followers_FollowerId",
                table: "Followers");

            migrationBuilder.DropColumn(
                name: "FollowerId",
                table: "Followers");
        }
    }
}
