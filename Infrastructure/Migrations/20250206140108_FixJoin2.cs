using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixJoin2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupEntityId",
                table: "FileGroupShares",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileGroupShares_GroupEntityId",
                table: "FileGroupShares",
                column: "GroupEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroupShares_Groups_GroupEntityId",
                table: "FileGroupShares",
                column: "GroupEntityId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileGroupShares_Groups_GroupEntityId",
                table: "FileGroupShares");

            migrationBuilder.DropIndex(
                name: "IX_FileGroupShares_GroupEntityId",
                table: "FileGroupShares");

            migrationBuilder.DropColumn(
                name: "GroupEntityId",
                table: "FileGroupShares");
        }
    }
}
