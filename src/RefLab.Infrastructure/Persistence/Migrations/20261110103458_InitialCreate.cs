using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefLab.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Abbreviations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Meaning = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abbreviations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Antibiotics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ScientificName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CultureType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CommonNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAllowedInPregnancy = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowedInChildren = table.Column<bool>(type: "bit", nullable: false),
                    Sensitivity = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    SensitivityUnit = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Antibiotics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    OperatorName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClinicalConstants",
                columns: table => new
                {
                    GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalConstants", x => x.GroupName);
                });

            migrationBuilder.CreateTable(
                name: "ClinicalSubGroups",
                columns: table => new
                {
                    Code = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleteProtected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalSubGroups", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ConversionFactors",
                columns: table => new
                {
                    TestSimpleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    FromUnit = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ToUnit = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Factor = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    FromDecimals = table.Column<int>(type: "int", nullable: false),
                    ToDecimals = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionFactors", x => new { x.TestSimpleName, x.FromUnit, x.ToUnit });
                });

            migrationBuilder.CreateTable(
                name: "Farms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LaboratorySettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PaperSize = table.Column<int>(type: "int", nullable: false),
                    ReportSequenceStart = table.Column<int>(type: "int", nullable: false),
                    IsBarcodeAutoCreated = table.Column<bool>(type: "bit", nullable: false),
                    IsWorkSheetPrinted = table.Column<bool>(type: "bit", nullable: false),
                    HistoryDisplayMode = table.Column<int>(type: "int", nullable: false),
                    GlobalHeaderText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    GlobalHeaderSecondaryText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    LeftMargin = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    RightMargin = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    TopMargin = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    BottomMargin = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    BackgroundColour = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    DefaultAccountType = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ConnectionReceiptHeader = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ConnectionReceiptFooter = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Printers = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratorySettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RoleCaption = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    IsAdministrator = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SecurityWordHash = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ShowTip = table.Column<bool>(type: "bit", nullable: false),
                    TipNumber = table.Column<int>(type: "int", nullable: false),
                    SpecialLimit = table.Column<int>(type: "int", nullable: false),
                    AttendanceStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AttendanceEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsOnBreak = table.Column<bool>(type: "bit", nullable: false),
                    BreakStartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastVisit = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Sex = table.Column<int>(type: "int", nullable: false),
                    Age = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    AgeUnit = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferringPhysicianCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ReceptionNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AdditionalInfo2 = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceLists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    TestSimpleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PriceListName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProthrombinControlCalibrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ControlTime = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    Concentration = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProthrombinControlCalibrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TestCode = table.Column<int>(type: "int", nullable: false),
                    TestSimpleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SubGroupCode = table.Column<int>(type: "int", nullable: false),
                    SexScope = table.Column<int>(type: "int", nullable: false),
                    AgeBand_From = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    AgeBand_To = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    AgeBand_Unit = table.Column<int>(type: "int", nullable: false),
                    PregnancySmokingFactor = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    Interval_LowLimit = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    Interval_HighLimit = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    Interval_LowExceptionText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Interval_HighExceptionText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Interval_LowComment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Interval_HighComment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Interval_RangeText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Interval_Unit = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Interval_CriticalInterval_LowCriticalLimit = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    Interval_CriticalInterval_HighCriticalLimit = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    Interval_CriticalInterval_CriticalRangeText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Interval_CriticalInterval_CriticalExceptionText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Interval_CriticalInterval_CriticalComment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ReferenceRangeType = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferringParties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ContactPersonCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ContactPersonName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    City = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSpecial = table.Column<bool>(type: "bit", nullable: false),
                    SpecialLimit = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CommissionRate_Value = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    Morsel = table.Column<int>(type: "int", nullable: true),
                    PriceMethod = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    PrintOrderNumber = table.Column<bool>(type: "bit", nullable: false),
                    TreatingPhysicianCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    TreatingPhysicianId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferringParties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestDefinitions",
                columns: table => new
                {
                    Code = table.Column<int>(type: "int", nullable: false),
                    BillingCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ArrangementNumber = table.Column<int>(type: "int", nullable: false),
                    SubGroupCode = table.Column<int>(type: "int", nullable: false),
                    SimpleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    WorkSheetName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    WorkSheetName2 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    HistoryName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BarcodeSymbol = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    SubGroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LogGroup = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSendOut = table.Column<bool>(type: "bit", nullable: false),
                    OtherLabName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    OtherLabPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PatientPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    LabPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AllowsQuestion = table.Column<bool>(type: "bit", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Effect = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Usage = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    SampleType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleteProtected = table.Column<bool>(type: "bit", nullable: false),
                    TestTime = table.Column<int>(type: "int", nullable: true),
                    PrintWithGroup = table.Column<bool>(type: "bit", nullable: false),
                    Barcode1 = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Barcode2 = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Barcode3 = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    SeeReport = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    NormalType = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    AddOrderNumber = table.Column<bool>(type: "bit", nullable: false),
                    HasTestOrderNumber = table.Column<bool>(type: "bit", nullable: false),
                    ReferenceRangeType = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    CriticalInterval_LowCriticalLimit = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    CriticalInterval_HighCriticalLimit = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    CriticalInterval_CriticalRangeText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CriticalInterval_CriticalExceptionText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CriticalInterval_CriticalComment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestDefinitions", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TestUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreatingPhysicians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatingPhysicians", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FarmTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FarmId = table.Column<int>(type: "int", nullable: false),
                    FarmName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TransactionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IssuedAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ReceivedAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FarmTransactions_Farms_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CultureResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CultureId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Organism = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SecondOrganism = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Colony = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Reactions = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Susceptibilities = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CultureResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CultureResults_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientHistories",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Selector = table.Column<int>(type: "int", nullable: false),
                    Slots = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PrintFlag = table.Column<bool>(type: "bit", nullable: false),
                    UsedSlotCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientHistories", x => new { x.PatientId, x.Selector });
                    table.ForeignKey(
                        name: "FK_PatientHistories_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LegacyIdentifier = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    RequestDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    OutDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TimeIn = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    PriceMethod = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    TotalPrice_Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TotalPrice_CurrencyCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    TotalPriceAfterDiscount_Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TotalPriceAfterDiscount_CurrencyCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    CommissionBaseAmount_Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CommissionBaseAmount_CurrencyCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Discount_Value = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    Discount100 = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    ReferringPartyId = table.Column<int>(type: "int", nullable: true),
                    TreatingPhysicianCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    ReceptionNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AdditionalInfo2 = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientOrders_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientPictures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TestCode = table.Column<int>(type: "int", nullable: true),
                    Slot = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientPictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientPictures_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestCode = table.Column<int>(type: "int", nullable: false),
                    TestSimpleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TestAbbreviation = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    TestName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    HistoryName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TestUnit = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    SubGroupCode = table.Column<int>(type: "int", nullable: false),
                    MainTestCode = table.Column<int>(type: "int", nullable: true),
                    ArrangementNumber = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Result_Text = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Result_NumericValue = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    Result_Unit = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    AdditionalResults = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceSnapshot_TestCode = table.Column<int>(type: "int", nullable: true),
                    ReferenceSnapshot_TestSimpleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceSnapshot_SexScope = table.Column<int>(type: "int", nullable: true),
                    ReferenceSnapshot_AgeBand_From = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    ReferenceSnapshot_AgeBand_To = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    ReferenceSnapshot_AgeBand_Unit = table.Column<int>(type: "int", nullable: true),
                    ReferenceSnapshot_Interval_LowLimit = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    ReferenceSnapshot_Interval_HighLimit = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    ReferenceSnapshot_Interval_LowExceptionText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ReferenceSnapshot_Interval_HighExceptionText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ReferenceSnapshot_Interval_LowComment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ReferenceSnapshot_Interval_HighComment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ReferenceSnapshot_Interval_RangeText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ReferenceSnapshot_Interval_Unit = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ReferenceSnapshot_Interval_CriticalInterval_LowCriticalLimit = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    ReferenceSnapshot_Interval_CriticalInterval_HighCriticalLimit = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    ReferenceSnapshot_Interval_CriticalInterval_CriticalRangeText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ReferenceSnapshot_Interval_CriticalInterval_CriticalExceptionText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ReferenceSnapshot_Interval_CriticalInterval_CriticalComment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ReferenceSnapshot_CapturedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsResultFinished = table.Column<bool>(type: "bit", nullable: false),
                    IsResultReviewed = table.Column<bool>(type: "bit", nullable: false),
                    IsResultPrinted = table.Column<bool>(type: "bit", nullable: false),
                    IsGiven = table.Column<bool>(type: "bit", nullable: false),
                    IsSentOut = table.Column<bool>(type: "bit", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    PrintWithGroup = table.Column<bool>(type: "bit", nullable: false),
                    ResultCasePrint = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Price_Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Price_CurrencyCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    OutPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    OutPriceBeforeDiscount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    OtherLabName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TestTime = table.Column<int>(type: "int", nullable: true),
                    Timeout = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    TakeTime = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    LogGroup = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EditedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PreviousUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PreviousAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PrintedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PrintedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PrintNumber = table.Column<long>(type: "bigint", nullable: true),
                    HandedOverBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    HandedOverAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PatientOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTests_PatientOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "PatientOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTests_PatientOrders_PatientOrderId",
                        column: x => x.PatientOrderId,
                        principalTable: "PatientOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LegacyOrderIdentifier = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    PaidDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReferringPartyCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    TestSimpleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Amount_Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Amount_CurrencyCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    TreatingPhysicianCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    RecordedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Number = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_PatientOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "PatientOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PrintedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintAudits_OrderTests_OrderTestId",
                        column: x => x.OrderTestId,
                        principalTable: "OrderTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SampleDispositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    DestinationLabName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RecordedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RecordedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleDispositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SampleDispositions_OrderTests_OrderTestId",
                        column: x => x.OrderTestId,
                        principalTable: "OrderTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SendOutRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationLabName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsDispatched = table.Column<bool>(type: "bit", nullable: false),
                    IsSettled = table.Column<bool>(type: "bit", nullable: false),
                    DispatchedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SettledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OutPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    OutPriceBeforeDiscount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendOutRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SendOutRequests_OrderTests_OrderTestId",
                        column: x => x.OrderTestId,
                        principalTable: "OrderTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Antibiotics_Symbol",
                table: "Antibiotics",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceEntries_OccurredAt",
                table: "AttendanceEntries",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceEntries_OperatorId",
                table: "AttendanceEntries",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CultureResults_PatientId",
                table: "CultureResults",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Farms_Name",
                table: "Farms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FarmTransactions_FarmId",
                table: "FarmTransactions",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_Operators_Name",
                table: "Operators",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTests_OrderId",
                table: "OrderTests",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTests_PatientOrderId",
                table: "OrderTests",
                column: "PatientOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTests_TestCode",
                table: "OrderTests",
                column: "TestCode");

            migrationBuilder.CreateIndex(
                name: "IX_PatientOrders_PatientId",
                table: "PatientOrders",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientOrders_RequestDate",
                table: "PatientOrders",
                column: "RequestDate");

            migrationBuilder.CreateIndex(
                name: "IX_PatientPictures_PatientId",
                table: "PatientPictures",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Name",
                table: "Patients",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaidDate",
                table: "Payments",
                column: "PaidDate");

            migrationBuilder.CreateIndex(
                name: "IX_PriceLists_TestSimpleName_PriceListName",
                table: "PriceLists",
                columns: new[] { "TestSimpleName", "PriceListName" });

            migrationBuilder.CreateIndex(
                name: "IX_PrintAudits_OrderTestId",
                table: "PrintAudits",
                column: "OrderTestId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAudits_PrintedAt",
                table: "PrintAudits",
                column: "PrintedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceRanges_TestCode_SubGroupCode_SexScope",
                table: "ReferenceRanges",
                columns: new[] { "TestCode", "SubGroupCode", "SexScope" });

            migrationBuilder.CreateIndex(
                name: "IX_ReferringParties_Code",
                table: "ReferringParties",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleDispositions_OrderTestId",
                table: "SampleDispositions",
                column: "OrderTestId");

            migrationBuilder.CreateIndex(
                name: "IX_SendOutRequests_OrderTestId",
                table: "SendOutRequests",
                column: "OrderTestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestDefinitions_SimpleName",
                table: "TestDefinitions",
                column: "SimpleName");

            migrationBuilder.CreateIndex(
                name: "IX_TestDefinitions_SubGroupCode",
                table: "TestDefinitions",
                column: "SubGroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_TreatingPhysicians_Code",
                table: "TreatingPhysicians",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Abbreviations");

            migrationBuilder.DropTable(
                name: "Antibiotics");

            migrationBuilder.DropTable(
                name: "AttendanceEntries");

            migrationBuilder.DropTable(
                name: "ClinicalConstants");

            migrationBuilder.DropTable(
                name: "ClinicalSubGroups");

            migrationBuilder.DropTable(
                name: "ConversionFactors");

            migrationBuilder.DropTable(
                name: "CultureResults");

            migrationBuilder.DropTable(
                name: "FarmTransactions");

            migrationBuilder.DropTable(
                name: "LaboratorySettings");

            migrationBuilder.DropTable(
                name: "Operators");

            migrationBuilder.DropTable(
                name: "PatientHistories");

            migrationBuilder.DropTable(
                name: "PatientPictures");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PriceLists");

            migrationBuilder.DropTable(
                name: "PrintAudits");

            migrationBuilder.DropTable(
                name: "ProthrombinControlCalibrations");

            migrationBuilder.DropTable(
                name: "ReferenceRanges");

            migrationBuilder.DropTable(
                name: "ReferringParties");

            migrationBuilder.DropTable(
                name: "SampleDispositions");

            migrationBuilder.DropTable(
                name: "SendOutRequests");

            migrationBuilder.DropTable(
                name: "TestDefinitions");

            migrationBuilder.DropTable(
                name: "TestUnits");

            migrationBuilder.DropTable(
                name: "TreatingPhysicians");

            migrationBuilder.DropTable(
                name: "Farms");

            migrationBuilder.DropTable(
                name: "OrderTests");

            migrationBuilder.DropTable(
                name: "PatientOrders");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
