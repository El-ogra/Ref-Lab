using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefLab.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReportBrandingAndSampleOrigin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SampleTakenOutsideLab",
                table: "OrderTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DoctorName",
                table: "LaboratorySettings",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorSignature",
                table: "LaboratorySettings",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeadingMode",
                table: "LaboratorySettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LabLogoPath",
                table: "LaboratorySettings",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogoHeightPx",
                table: "LaboratorySettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogoWidthPx",
                table: "LaboratorySettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportLogoPath",
                table: "LaboratorySettings",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThanksText",
                table: "LaboratorySettings",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleTakenOutsideLab",
                table: "OrderTests");

            migrationBuilder.DropColumn(
                name: "DoctorName",
                table: "LaboratorySettings");

            migrationBuilder.DropColumn(
                name: "DoctorSignature",
                table: "LaboratorySettings");

            migrationBuilder.DropColumn(
                name: "HeadingMode",
                table: "LaboratorySettings");

            migrationBuilder.DropColumn(
                name: "LabLogoPath",
                table: "LaboratorySettings");

            migrationBuilder.DropColumn(
                name: "LogoHeightPx",
                table: "LaboratorySettings");

            migrationBuilder.DropColumn(
                name: "LogoWidthPx",
                table: "LaboratorySettings");

            migrationBuilder.DropColumn(
                name: "ReportLogoPath",
                table: "LaboratorySettings");

            migrationBuilder.DropColumn(
                name: "ThanksText",
                table: "LaboratorySettings");
        }
    }
}
