using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneyCounter.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoundersTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Subcategory = table.Column<string>(type: "TEXT", nullable: true),
                    MoneyAmount = table.Column<string>(type: "TEXT", nullable: true),
                    DatetimeOfFinish = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    FromWallet = table.Column<string>(type: "TEXT", nullable: true),
                    ToWallet = table.Column<string>(type: "TEXT", nullable: true),
                    SumOnSourceWallet = table.Column<string>(type: "TEXT", nullable: true),
                    SumOnAimWallet = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundersTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionCategories",
                columns: table => new
                {
                    CategoryKey = table.Column<string>(type: "TEXT", nullable: false),
                    SubCategoryKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategories", x => new { x.CategoryKey, x.SubCategoryKey });
                });

            migrationBuilder.CreateTable(
                name: "TransactionIDsForDeletion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransactionId = table.Column<int>(type: "INTEGER", nullable: false),
                    OperatorThatRequestsId = table.Column<int>(type: "INTEGER", nullable: false),
                    OperatorThatRequests = table.Column<string>(type: "TEXT", nullable: true),
                    AdminThatAllowedId = table.Column<int>(type: "INTEGER", nullable: true),
                    AdminThatAllowed = table.Column<string>(type: "TEXT", nullable: true),
                    WasDeletionAllowed = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionIDsForDeletion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Subcategory = table.Column<string>(type: "TEXT", nullable: true),
                    MoneyAmount = table.Column<string>(type: "TEXT", nullable: true),
                    DatetimeOfFinish = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsGain = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsStart = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFinal = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsReversal = table.Column<bool>(type: "INTEGER", nullable: false),
                    FromWallet = table.Column<string>(type: "TEXT", nullable: true),
                    ToWallet = table.Column<string>(type: "TEXT", nullable: true),
                    SumOnSourceWallet = table.Column<string>(type: "TEXT", nullable: true),
                    SumOnAimWallet = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMainMenu = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    ChatStatus = table.Column<string>(type: "TEXT", nullable: true),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "WhiteList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhiteList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BalanceMultiplications",
                columns: table => new
                {
                    BMId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransactionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdjunctionSum = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceMultiplications", x => x.BMId);
                    table.ForeignKey(
                        name: "FK_BalanceMultiplications_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceMultiplications_TransactionId",
                table: "BalanceMultiplications",
                column: "TransactionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceMultiplications");

            migrationBuilder.DropTable(
                name: "FoundersTransactions");

            migrationBuilder.DropTable(
                name: "TransactionCategories");

            migrationBuilder.DropTable(
                name: "TransactionIDsForDeletion");

            migrationBuilder.DropTable(
                name: "UserMessages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WhiteList");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
