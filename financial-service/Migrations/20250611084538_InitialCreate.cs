using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bonus",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    bonus_image = table.Column<string>(type: "text", nullable: true),
                    image_for_deposit_view = table.Column<string>(type: "text", nullable: true),
                    bonus_type = table.Column<int>(type: "integer", nullable: false),
                    drop_chance = table.Column<decimal>(type: "numeric", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    bonus_type1 = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: true),
                    cashback_percentage = table.Column<decimal>(type: "numeric", nullable: true),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    deposit_cap = table.Column<decimal>(type: "numeric", nullable: true),
                    bonus_multiplier = table.Column<decimal>(type: "numeric", nullable: true),
                    multiplier_type = table.Column<int>(type: "integer", nullable: true),
                    discount_percentage = table.Column<decimal>(type: "numeric", nullable: true),
                    case_bonus_type_discount = table.Column<int>(type: "integer", nullable: true),
                    case_count = table.Column<int>(type: "integer", nullable: true),
                    minimum_deposit = table.Column<decimal>(type: "numeric", nullable: true),
                    item_count = table.Column<int>(type: "integer", nullable: true),
                    item_min_cost = table.Column<decimal>(type: "numeric", nullable: true),
                    item_max_cost = table.Column<decimal>(type: "numeric", nullable: true),
                    is_deposit_dependent = table.Column<bool>(type: "boolean", nullable: true),
                    letter = table.Column<string>(type: "text", nullable: true),
                    extra_spins = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bonus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "financial_data",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    current_balance = table.Column<decimal>(type: "numeric", nullable: false),
                    bonus_balance = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_data", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    financial_data_id = table.Column<string>(type: "text", nullable: true),
                    balance_before = table.Column<decimal>(type: "numeric", nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    payment_type = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaction_financial_data_financial_data_id",
                        column: x => x.financial_data_id,
                        principalTable: "financial_data",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "users_bonuses",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    fin_data_id = table.Column<string>(type: "text", nullable: false),
                    bonus_id = table.Column<string>(type: "text", nullable: false),
                    is_wheel_bonus = table.Column<bool>(type: "boolean", nullable: false),
                    time_got_bonus = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_bonuses", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_bonuses_bonus_bonus_id",
                        column: x => x.bonus_id,
                        principalTable: "bonus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_bonuses_financial_data_fin_data_id",
                        column: x => x.fin_data_id,
                        principalTable: "financial_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_financial_data_user_id",
                table: "financial_data",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transaction_financial_data_id",
                table: "transaction",
                column: "financial_data_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_bonuses_bonus_id",
                table: "users_bonuses",
                column: "bonus_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_bonuses_fin_data_id",
                table: "users_bonuses",
                column: "fin_data_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "users_bonuses");

            migrationBuilder.DropTable(
                name: "bonus");

            migrationBuilder.DropTable(
                name: "financial_data");
        }
    }
}
