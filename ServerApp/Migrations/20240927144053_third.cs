using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerApp.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DocumentStatuses_StatusId",
                table: "DocumentStatuses",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentStatuses_Statuses_StatusId",
                table: "DocumentStatuses",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentStatuses_Statuses_StatusId",
                table: "DocumentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_DocumentStatuses_StatusId",
                table: "DocumentStatuses");
        }
    }
}
