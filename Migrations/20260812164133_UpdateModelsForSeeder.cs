using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quantify.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsForSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEC3l6G992jUBBhCMy0mw+aZ44tpfIpaSzTgo7H6/sJY4ne8exn4uAfVkVzNEsMgqUg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHbI96ZPuneq+YTE8LIVhmHwLfg1qgISOg/LPJL4JhT9FEp/SgQY5NiJTvH4CogDsA==");
        }
    }
}
