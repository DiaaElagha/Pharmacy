using Microsoft.EntityFrameworkCore.Migrations;

namespace Pharmacy.Data.Migrations
{
    public partial class edittablePaymnet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Quntity",
                table: "Payment",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Total",
                table: "Payment",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quntity",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Payment");
        }
    }
}
