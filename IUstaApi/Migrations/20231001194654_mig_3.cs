using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IUstaApi.Migrations
{
    /// <inheritdoc />
    public partial class mig_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerCategory_AspNetUsers_WorkerId",
                table: "WorkerCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerCategory_Categories_CategoryId",
                table: "WorkerCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerCategory",
                table: "WorkerCategory");

            migrationBuilder.RenameTable(
                name: "WorkerCategory",
                newName: "WorkerCategories");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerCategory_WorkerId",
                table: "WorkerCategories",
                newName: "IX_WorkerCategories_WorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerCategory_CategoryId",
                table: "WorkerCategories",
                newName: "IX_WorkerCategories_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerCategories",
                table: "WorkerCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerCategories_AspNetUsers_WorkerId",
                table: "WorkerCategories",
                column: "WorkerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerCategories_Categories_CategoryId",
                table: "WorkerCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerCategories_AspNetUsers_WorkerId",
                table: "WorkerCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerCategories_Categories_CategoryId",
                table: "WorkerCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerCategories",
                table: "WorkerCategories");

            migrationBuilder.RenameTable(
                name: "WorkerCategories",
                newName: "WorkerCategory");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerCategories_WorkerId",
                table: "WorkerCategory",
                newName: "IX_WorkerCategory_WorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerCategories_CategoryId",
                table: "WorkerCategory",
                newName: "IX_WorkerCategory_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerCategory",
                table: "WorkerCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerCategory_AspNetUsers_WorkerId",
                table: "WorkerCategory",
                column: "WorkerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerCategory_Categories_CategoryId",
                table: "WorkerCategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
