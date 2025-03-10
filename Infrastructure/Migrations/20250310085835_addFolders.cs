using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_FolderEntity_FolderId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_FolderEntity_Users_OwnerId",
                table: "FolderEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FolderEntity",
                table: "FolderEntity");

            migrationBuilder.RenameTable(
                name: "FolderEntity",
                newName: "Folders");

            migrationBuilder.RenameIndex(
                name: "IX_FolderEntity_OwnerId",
                table: "Folders",
                newName: "IX_Folders_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Folders",
                table: "Folders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Folders_FolderId",
                table: "Files",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Users_OwnerId",
                table: "Folders",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Folders_FolderId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Users_OwnerId",
                table: "Folders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Folders",
                table: "Folders");

            migrationBuilder.RenameTable(
                name: "Folders",
                newName: "FolderEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Folders_OwnerId",
                table: "FolderEntity",
                newName: "IX_FolderEntity_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FolderEntity",
                table: "FolderEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_FolderEntity_FolderId",
                table: "Files",
                column: "FolderId",
                principalTable: "FolderEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FolderEntity_Users_OwnerId",
                table: "FolderEntity",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
