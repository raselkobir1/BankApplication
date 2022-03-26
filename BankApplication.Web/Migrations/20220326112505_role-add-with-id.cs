using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApplication.Web.Migrations
{
    public partial class roleaddwithid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "22c0ede2-676c-4a3d-8d0f-dad4c391afee");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "d44dde4f-b91c-4021-99b4-30fb3143bd05");

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1", "bd1c90a6-6708-4e73-a954-fc6022aa4c2c", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2", "aba4381f-6af3-4178-9746-7eeaf40570d2", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "22c0ede2-676c-4a3d-8d0f-dad4c391afee", "d13d921b-a5a4-4068-89cb-7408e9826e6a", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d44dde4f-b91c-4021-99b4-30fb3143bd05", "516dd579-669d-4fa6-a67e-d8ef28bb9d61", "Administrator", "ADMINISTRATOR" });
        }
    }
}
