using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpensesControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToExpenseTypeAndMoneyFund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MoneyFunds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ExpenseTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MoneyFunds_UserId",
                table: "MoneyFunds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTypes_UserId",
                table: "ExpenseTypes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseTypes_Users_UserId",
                table: "ExpenseTypes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MoneyFunds_Users_UserId",
                table: "MoneyFunds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseTypes_Users_UserId",
                table: "ExpenseTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_MoneyFunds_Users_UserId",
                table: "MoneyFunds");

            migrationBuilder.DropIndex(
                name: "IX_MoneyFunds_UserId",
                table: "MoneyFunds");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseTypes_UserId",
                table: "ExpenseTypes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MoneyFunds");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ExpenseTypes");
        }
    }
}
