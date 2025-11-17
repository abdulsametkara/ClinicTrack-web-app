using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinickDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciTablosuGuncelleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoğumTarihi",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "Soyisim",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "TCNo",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "TelefonNumarası",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "İsim",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Doktorlar");

            migrationBuilder.DropColumn(
                name: "Soyisim",
                table: "Doktorlar");

            migrationBuilder.DropColumn(
                name: "TelefonNumarası",
                table: "Doktorlar");

            migrationBuilder.DropColumn(
                name: "İsim",
                table: "Doktorlar");

            migrationBuilder.RenameColumn(
                name: "ParolaHash",
                table: "Kullanıcılar",
                newName: "TCNo");

            migrationBuilder.AddColumn<DateTime>(
                name: "DoğumTarihi",
                table: "Kullanıcılar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OluşturulmaTarihi",
                table: "Kullanıcılar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parola",
                table: "Kullanıcılar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TelefonNumarası",
                table: "Kullanıcılar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UzmanlıkId",
                table: "Kullanıcılar",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cinsiyet",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AcilDurumKişisi",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcilDurumTelefon",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adres",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KanGrubu",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KullanıcıId",
                table: "Hastalar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DiplomaNo",
                table: "Doktorlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KullanıcıId",
                table: "Doktorlar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "MezuniyetTarihi",
                table: "Doktorlar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ünvan",
                table: "Doktorlar",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoğumTarihi",
                table: "Kullanıcılar");

            migrationBuilder.DropColumn(
                name: "OluşturulmaTarihi",
                table: "Kullanıcılar");

            migrationBuilder.DropColumn(
                name: "Parola",
                table: "Kullanıcılar");

            migrationBuilder.DropColumn(
                name: "TelefonNumarası",
                table: "Kullanıcılar");

            migrationBuilder.DropColumn(
                name: "UzmanlıkId",
                table: "Kullanıcılar");

            migrationBuilder.DropColumn(
                name: "AcilDurumKişisi",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "AcilDurumTelefon",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "Adres",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "KanGrubu",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "KullanıcıId",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "DiplomaNo",
                table: "Doktorlar");

            migrationBuilder.DropColumn(
                name: "KullanıcıId",
                table: "Doktorlar");

            migrationBuilder.DropColumn(
                name: "MezuniyetTarihi",
                table: "Doktorlar");

            migrationBuilder.DropColumn(
                name: "Ünvan",
                table: "Doktorlar");

            migrationBuilder.RenameColumn(
                name: "TCNo",
                table: "Kullanıcılar",
                newName: "ParolaHash");

            migrationBuilder.AlterColumn<string>(
                name: "Cinsiyet",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DoğumTarihi",
                table: "Hastalar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Soyisim",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TCNo",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TelefonNumarası",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "İsim",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Doktorlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Soyisim",
                table: "Doktorlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TelefonNumarası",
                table: "Doktorlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "İsim",
                table: "Doktorlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
