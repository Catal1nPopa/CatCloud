using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserRelationInRequestHelp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "RequestHelps",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RequestHelps_UserId1",
                table: "RequestHelps",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHelps_Users_UserId1",
                table: "RequestHelps",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestHelps_Users_UserId1",
                table: "RequestHelps");

            migrationBuilder.DropIndex(
                name: "IX_RequestHelps_UserId1",
                table: "RequestHelps");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "RequestHelps");
        }
    }
}
