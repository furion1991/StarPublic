using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "base_log",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    performed_by_id = table.Column<string>(type: "text", nullable: true),
                    date_of_log = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_base_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "daily_server_stats",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    cases_opened = table.Column<int>(type: "integer", nullable: false),
                    statistics_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deposits_today = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_server_stats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "opened_cases",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    case_id = table.Column<string>(type: "text", nullable: false),
                    item_dropped_id = table.Column<string>(type: "text", nullable: false),
                    time_opened = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_opened_id = table.Column<string>(type: "text", nullable: false),
                    item_cost = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opened_cases", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "case_log",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    case_id = table.Column<string>(type: "text", nullable: false),
                    case_log_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_case_log_base_log_id",
                        column: x => x.id,
                        principalTable: "base_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "financial_log",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    financial_record_id = table.Column<string>(type: "text", nullable: false),
                    financial_log_type = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_financial_log_base_log_id",
                        column: x => x.id,
                        principalTable: "base_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_log",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    item_id = table.Column<string>(type: "text", nullable: false),
                    item_log_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_log_base_log_id",
                        column: x => x.id,
                        principalTable: "base_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_log",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    user_log_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_log_base_log_id",
                        column: x => x.id,
                        principalTable: "base_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "case_log");

            migrationBuilder.DropTable(
                name: "daily_server_stats");

            migrationBuilder.DropTable(
                name: "financial_log");

            migrationBuilder.DropTable(
                name: "item_log");

            migrationBuilder.DropTable(
                name: "opened_cases");

            migrationBuilder.DropTable(
                name: "user_log");

            migrationBuilder.DropTable(
                name: "base_log");
        }
    }
}
