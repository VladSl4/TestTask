using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerApp.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Statuses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "DocumentStatuses",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DocumentStatusId",
                table: "DocumentStatuses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "Documents",
                newName: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentStatuses_DocumentId",
                table: "DocumentStatuses",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentStatuses_Documents_DocumentId",
                table: "DocumentStatuses",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentStatuses_Documents_DocumentId",
                table: "DocumentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_DocumentStatuses_DocumentId",
                table: "DocumentStatuses");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Statuses",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "DocumentStatuses",
                newName: "DateTime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DocumentStatuses",
                newName: "DocumentStatusId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Documents",
                newName: "DocumentId");
        }
    }
}
