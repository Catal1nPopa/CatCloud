using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixJoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserEntityId",
                table: "FileUserShares",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileUserShares_UserEntityId",
                table: "FileUserShares",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUserShares_Users_UserEntityId",
                table: "FileUserShares",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUserShares_Users_UserEntityId",
                table: "FileUserShares");

            migrationBuilder.DropIndex(
                name: "IX_FileUserShares_UserEntityId",
                table: "FileUserShares");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "FileUserShares");
        }
    }
}
