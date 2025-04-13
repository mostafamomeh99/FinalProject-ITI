using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseConnection.Migrations
{
    /// <inheritdoc />
    public partial class _5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TripSiteDetails",
                columns: table => new
                {
                    TripId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TripName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TripDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TripIsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AvaiblePeople = table.Column<int>(type: "int", nullable: false),
                    TripTotalDuration = table.Column<int>(type: "int", nullable: false),
                    TripendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TripStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TripMaxPeople = table.Column<int>(type: "int", nullable: false),
                    TripMoney = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TripisOutOfDate = table.Column<bool>(type: "bit", nullable: false),
                    TransportId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncludedId = table.Column<int>(type: "int", nullable: false),
                    IncludedItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExcludedId = table.Column<int>(type: "int", nullable: false),
                    ExcludedItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagId = table.Column<int>(type: "int", nullable: false),
                    SiteImageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanOfTripInSite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GovernmentId = table.Column<int>(type: "int", nullable: false),
                    GovernmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GovernmentImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfDaysInSite = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripSiteDetails");
        }
    }
}
