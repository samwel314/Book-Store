using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Samuel_Web.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedCompanyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "Cairo", "Tech Solutions Ltd", "+20 100 123 4567", "11511", "Cairo", "12 El Tahrir St" },
                    { 2, "Giza", "Nile Soft", "+20 109 555 8821", "12611", "Giza", "45 Corniche El Nile" },
                    { 3, "Mansoura", "Delta Systems", "+20 122 334 9988", "35511", "Dakahlia", "10 Saad Zaghloul St" },
                    { 4, "Alexandria", "Alexandria Trading", "+20 111 777 6633", "21500", "Alexandria", "22 El Horreya Rd" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
