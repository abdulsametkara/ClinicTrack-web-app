using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinickDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UzmanlıkTablosuEkleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UzmanlıkAlanı",
                table: "Doktorlar");

            migrationBuilder.AddColumn<string>(
                name: "DoktorNotları",
                table: "Randevular",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HastaŞikayeti",
                table: "Randevular",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UzmanlıkId",
                table: "Randevular",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UzmanlıkId",
                table: "Doktorlar",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoktorNotları",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "HastaŞikayeti",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "UzmanlıkId",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "UzmanlıkId",
                table: "Doktorlar");

            migrationBuilder.AddColumn<string>(
                name: "UzmanlıkAlanı",
                table: "Doktorlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
