using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasesService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "case",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    image = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: true),
                    current_open = table.Column<int>(type: "integer", nullable: false),
                    open_limit = table.Column<int>(type: "integer", nullable: true),
                    discount = table.Column<float>(type: "real", nullable: true),
                    old_price = table.Column<decimal>(type: "numeric", nullable: true),
                    accumulated_profit = table.Column<decimal>(type: "numeric", nullable: true),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    alpha = table.Column<float>(type: "real", nullable: false),
                    bonus_new_user_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    bonus_new_user_rolls = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    rarity = table.Column<int>(type: "integer", nullable: false),
                    available_for_upgrade = table.Column<bool>(type: "boolean", nullable: false),
                    base_cost = table.Column<decimal>(type: "numeric", nullable: true),
                    sell_price = table.Column<decimal>(type: "numeric", nullable: true),
                    is_visible = table.Column<bool>(type: "boolean", nullable: true),
                    image = table.Column<string>(type: "text", nullable: false),
                    game = table.Column<string>(type: "text", nullable: true),
                    is_available_for_contract = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Multipliers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NewPlayerMultiplier = table.Column<double>(type: "double precision", nullable: false),
                    NewPlayerCasesRollCount = table.Column<int>(type: "integer", nullable: false),
                    NewPlayerUpgradesCount = table.Column<int>(type: "integer", nullable: false),
                    NewPlayerContractsCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Multipliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "item_case",
                columns: table => new
                {
                    case_id = table.Column<string>(type: "text", nullable: false),
                    item_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_case", x => new { x.case_id, x.item_id });
                    table.ForeignKey(
                        name: "FK_item_case_case_case_id",
                        column: x => x.case_id,
                        principalTable: "case",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_case_item_item_id",
                        column: x => x.item_id,
                        principalTable: "item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_item_case_case_id_item_id",
                table: "item_case",
                columns: new[] { "case_id", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_case_item_id",
                table: "item_case",
                column: "item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_case");

            migrationBuilder.DropTable(
                name: "Multipliers");

            migrationBuilder.DropTable(
                name: "case");

            migrationBuilder.DropTable(
                name: "item");
        }
    }
}
