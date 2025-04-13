using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseConnection.Migrations
{
    /// <inheritdoc />
    public partial class _4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Drivers_TransportationId",
                table: "Drivers");

            migrationBuilder.AddColumn<string>(
                name: "TransportationId",
                table: "Trips",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TransportationId",
                table: "Trips",
                column: "TransportationId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_TransportationId",
                table: "Drivers",
                column: "TransportationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Transportations_TransportationId",
                table: "Trips",
                column: "TransportationId",
                principalTable: "Transportations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Transportations_TransportationId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_TransportationId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_TransportationId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "TransportationId",
                table: "Trips");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_TransportationId",
                table: "Drivers",
                column: "TransportationId");
        }
    }
}
