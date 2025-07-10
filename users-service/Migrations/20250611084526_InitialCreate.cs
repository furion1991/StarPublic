using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prize_draws",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DrawDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PrizeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsFinished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prize_draws", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    image_path = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    BlockStatusId = table.Column<string>(type: "text", nullable: false),
                    chance_boost = table.Column<double>(type: "double precision", nullable: false),
                    UserRoleId = table.Column<string>(type: "text", nullable: false),
                    dateofregistration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UserInventoryId = table.Column<string>(type: "text", nullable: false),
                    UserStatisticsId = table.Column<string>(type: "text", nullable: false),
                    PriceDrawId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_prize_draws_PriceDrawId",
                        column: x => x.PriceDrawId,
                        principalTable: "prize_draws",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "blockstatus",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    isblocked = table.Column<bool>(type: "boolean", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true),
                    performedbyid = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blockstatus", x => x.id);
                    table.ForeignKey(
                        name: "FK_blockstatus_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contracts_records",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ItemsFromIds = table.Column<string>(type: "jsonb", nullable: false),
                    ResultItemId = table.Column<string>(type: "text", nullable: false),
                    DateOfContract = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts_records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contracts_records_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "daily_bonuses",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    streak = table.Column<int>(type: "integer", nullable: false),
                    last_got_bonus = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_bonuses", x => x.id);
                    table.ForeignKey(
                        name: "FK_daily_bonuses_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prize_draw_results",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Winner = table.Column<string>(type: "text", nullable: true),
                    PrizeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DateDrawFinished = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prize_draw_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prize_draw_results_users_Winner",
                        column: x => x.Winner,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UpgradeHistoryRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ItemSpentId = table.Column<string>(type: "text", nullable: false),
                    ItemResultId = table.Column<string>(type: "text", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "boolean", nullable: false),
                    DateOfUpgrade = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpgradeHistoryRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UpgradeHistoryRecord_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userinventory",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    userid = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userinventory", x => x.id);
                    table.ForeignKey(
                        name: "FK_userinventory_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userrole",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    userid = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userrole", x => x.id);
                    table.ForeignKey(
                        name: "FK_userrole_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userstatistics",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    casesbought = table.Column<int>(type: "integer", nullable: false),
                    ordersplaced = table.Column<int>(type: "integer", nullable: false),
                    crashrocketsplayed = table.Column<int>(type: "integer", nullable: false),
                    luckbaraban = table.Column<int>(type: "integer", nullable: false),
                    promocodesused = table.Column<int>(type: "integer", nullable: false),
                    userid = table.Column<string>(type: "text", nullable: true),
                    fail_score = table.Column<int>(type: "integer", nullable: false),
                    total_cases_spent = table.Column<decimal>(type: "numeric", nullable: false),
                    total_cases_profit = table.Column<decimal>(type: "numeric", nullable: false),
                    contracts_placed = table.Column<int>(type: "integer", nullable: false),
                    total_contracts_spent = table.Column<decimal>(type: "numeric", nullable: false),
                    total_contracts_profit = table.Column<decimal>(type: "numeric", nullable: false),
                    upgrades_played = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userstatistics", x => x.id);
                    table.ForeignKey(
                        name: "FK_userstatistics_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_records_t",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_inventory_id = table.Column<string>(type: "text", nullable: true),
                    item_id = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false),
                    is_item_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_records_t", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_records_t_userinventory_user_inventory_id",
                        column: x => x.user_inventory_id,
                        principalTable: "userinventory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blockstatus_userid",
                table: "blockstatus",
                column: "userid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contracts_records_UserId",
                table: "contracts_records",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_bonuses_user_id",
                table: "daily_bonuses",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_records_t_user_inventory_id",
                table: "item_records_t",
                column: "user_inventory_id");

            migrationBuilder.CreateIndex(
                name: "IX_prize_draw_results_Winner",
                table: "prize_draw_results",
                column: "Winner");

            migrationBuilder.CreateIndex(
                name: "IX_UpgradeHistoryRecord_UserId",
                table: "UpgradeHistoryRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_userinventory_userid",
                table: "userinventory",
                column: "userid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userrole_userid",
                table: "userrole",
                column: "userid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_PriceDrawId",
                table: "users",
                column: "PriceDrawId");

            migrationBuilder.CreateIndex(
                name: "IX_userstatistics_userid",
                table: "userstatistics",
                column: "userid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blockstatus");

            migrationBuilder.DropTable(
                name: "contracts_records");

            migrationBuilder.DropTable(
                name: "daily_bonuses");

            migrationBuilder.DropTable(
                name: "item_records_t");

            migrationBuilder.DropTable(
                name: "prize_draw_results");

            migrationBuilder.DropTable(
                name: "UpgradeHistoryRecord");

            migrationBuilder.DropTable(
                name: "userrole");

            migrationBuilder.DropTable(
                name: "userstatistics");

            migrationBuilder.DropTable(
                name: "userinventory");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "prize_draws");
        }
    }
}
