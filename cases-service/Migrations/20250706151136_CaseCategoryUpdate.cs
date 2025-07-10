using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasesService.Migrations
{
    /// <inheritdoc />
    public partial class CaseCategoryUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "case",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CaseCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormilizedName = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_case_CategoryId",
                table: "case",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_case_CaseCategories_CategoryId",
                table: "case",
                column: "CategoryId",
                principalTable: "CaseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_case_CaseCategories_CategoryId",
                table: "case");

            migrationBuilder.DropTable(
                name: "CaseCategories");

            migrationBuilder.DropIndex(
                name: "IX_case_CategoryId",
                table: "case");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "case");
        }
    }
}
