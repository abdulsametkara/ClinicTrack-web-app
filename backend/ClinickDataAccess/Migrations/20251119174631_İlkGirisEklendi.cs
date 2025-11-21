using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinickDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class İlkGirisEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "İlkGiris",
                table: "Kullanıcılar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Kullanıcılar",
                keyColumn: "Id",
                keyValue: 1,
                column: "İlkGiris",
                value: true);

            migrationBuilder.UpdateData(
                table: "Uzmanlıklar",
                keyColumn: "Id",
                keyValue: 1,
                column: "RecordDate",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Uzmanlıklar",
                keyColumn: "Id",
                keyValue: 2,
                column: "RecordDate",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Uzmanlıklar",
                keyColumn: "Id",
                keyValue: 3,
                column: "RecordDate",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Uzmanlıklar",
                keyColumn: "Id",
                keyValue: 4,
                column: "RecordDate",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Uzmanlıklar",
                keyColumn: "Id",
                keyValue: 5,
                column: "RecordDate",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "İlkGiris",
                table: "Kullanıcılar");

            migrationBuilder.UpdateData(
                table: "Uzmanlıklar",
                keyColumn: "Id",
                keyValue: 1,
                column: "RecordDate",
                value: new DateTime(2025, 11, 19, 19, 24, 1, 963, DateTimeKind.Local).AddTicks(4648));

            migrationBuilder.UpdateData(
                table: "Uzmanlıklar",
                keyColumn: "Id",
                keyValue: 2,
                column: "RecordDate",
                value: new DateTime(2025, 11, 19, 19, 24, 1, 964, DateTimeKind.Local).AddTicks(4417));

            migrationBuilder.UpdateData(
                table: "Uzmanlıklar",
                keyColumn: "Id",
                keyValue: 3,
                column: "RecordDate",
                value: new DateTime(2025, 11, 19, 19, 24, 1, 964, DateTimeKind.Local).AddTicks(4431));

            migrationBuilder.UpdateData(
                table: "Uzmanlıklar",
                keyColumn: "Id",
                keyValue: 4,
                column: "RecordDate",
                value: new DateTime(2025, 11, 19, 19, 24, 1, 964, DateTimeKind.Local).AddTicks(4432));

            migrationBuilder.UpdateData(
                table: "Uzmanlıklar",
                keyColumn: "Id",
                keyValue: 5,
                column: "RecordDate",
                value: new DateTime(2025, 11, 19, 19, 24, 1, 964, DateTimeKind.Local).AddTicks(4433));
        }
    }
}
