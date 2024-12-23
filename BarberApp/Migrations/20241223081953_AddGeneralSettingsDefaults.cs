using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberApp.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralSettingsDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GeneralSettings",
                columns: new[] { "Id", "Description", "Keywords", "LogoUrl", "Name", "SeoDescription", "SeoTitle" },
                values: new object[] { 1, "Default Description", "Default, SEO, Keywords", "/images/default-logo.png", "Default Name", "Default SEO Description", "Default SEO Title" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GeneralSettings",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
