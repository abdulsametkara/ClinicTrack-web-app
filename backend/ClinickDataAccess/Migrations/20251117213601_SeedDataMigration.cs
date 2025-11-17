using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinickDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Kullanıcılar",
                columns: new[] { "Id", "DoğumTarihi", "Email", "OluşturulmaTarihi", "Parola", "RecordDate", "Rol", "Soyisim", "TCNo", "TelefonNumarası", "UzmanlıkId", "İsim" },
                values: new object[] { 1, null, "admin@clinicktrack.com", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "User", "12345678901", null, null, "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Kullanıcılar",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
