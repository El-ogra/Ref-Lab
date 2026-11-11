using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefLab.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReportFooterAndLayoutText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LayoutText",
                table: "LaboratorySettings",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportFooterText",
                table: "LaboratorySettings",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LayoutText",
                table: "LaboratorySettings");

            migrationBuilder.DropColumn(
                name: "ReportFooterText",
                table: "LaboratorySettings");
        }
    }
}
