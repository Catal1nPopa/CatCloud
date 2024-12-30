using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFileSharingEntitiess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileEntity_Users_UploadedByUserId",
                table: "FileEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_FileGroupShareEntity_FileEntity_FileId",
                table: "FileGroupShareEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_FileGroupShareEntity_Groups_GroupId",
                table: "FileGroupShareEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUserShareEntity_FileEntity_FileId",
                table: "FileUserShareEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUserShareEntity_Users_UserId",
                table: "FileUserShareEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileUserShareEntity",
                table: "FileUserShareEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileGroupShareEntity",
                table: "FileGroupShareEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileEntity",
                table: "FileEntity");

            migrationBuilder.RenameTable(
                name: "FileUserShareEntity",
                newName: "FileUserShares");

            migrationBuilder.RenameTable(
                name: "FileGroupShareEntity",
                newName: "FileGroupShares");

            migrationBuilder.RenameTable(
                name: "FileEntity",
                newName: "Files");

            migrationBuilder.RenameIndex(
                name: "IX_FileUserShareEntity_UserId",
                table: "FileUserShares",
                newName: "IX_FileUserShares_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FileGroupShareEntity_GroupId",
                table: "FileGroupShares",
                newName: "IX_FileGroupShares_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_FileEntity_UploadedByUserId",
                table: "Files",
                newName: "IX_Files_UploadedByUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileUserShares",
                table: "FileUserShares",
                columns: new[] { "FileId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileGroupShares",
                table: "FileGroupShares",
                columns: new[] { "FileId", "GroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroupShares_Files_FileId",
                table: "FileGroupShares",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroupShares_Groups_GroupId",
                table: "FileGroupShares",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_UploadedByUserId",
                table: "Files",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileUserShares_Files_FileId",
                table: "FileUserShares",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileUserShares_Users_UserId",
                table: "FileUserShares",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileGroupShares_Files_FileId",
                table: "FileGroupShares");

            migrationBuilder.DropForeignKey(
                name: "FK_FileGroupShares_Groups_GroupId",
                table: "FileGroupShares");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_UploadedByUserId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUserShares_Files_FileId",
                table: "FileUserShares");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUserShares_Users_UserId",
                table: "FileUserShares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileUserShares",
                table: "FileUserShares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileGroupShares",
                table: "FileGroupShares");

            migrationBuilder.RenameTable(
                name: "FileUserShares",
                newName: "FileUserShareEntity");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "FileEntity");

            migrationBuilder.RenameTable(
                name: "FileGroupShares",
                newName: "FileGroupShareEntity");

            migrationBuilder.RenameIndex(
                name: "IX_FileUserShares_UserId",
                table: "FileUserShareEntity",
                newName: "IX_FileUserShareEntity_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_UploadedByUserId",
                table: "FileEntity",
                newName: "IX_FileEntity_UploadedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_FileGroupShares_GroupId",
                table: "FileGroupShareEntity",
                newName: "IX_FileGroupShareEntity_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileUserShareEntity",
                table: "FileUserShareEntity",
                columns: new[] { "FileId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileEntity",
                table: "FileEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileGroupShareEntity",
                table: "FileGroupShareEntity",
                columns: new[] { "FileId", "GroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FileEntity_Users_UploadedByUserId",
                table: "FileEntity",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroupShareEntity_FileEntity_FileId",
                table: "FileGroupShareEntity",
                column: "FileId",
                principalTable: "FileEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileGroupShareEntity_Groups_GroupId",
                table: "FileGroupShareEntity",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileUserShareEntity_FileEntity_FileId",
                table: "FileUserShareEntity",
                column: "FileId",
                principalTable: "FileEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileUserShareEntity_Users_UserId",
                table: "FileUserShareEntity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
