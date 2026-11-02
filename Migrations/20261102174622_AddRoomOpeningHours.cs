using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyRoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomOpeningHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "ClosingTime",
                table: "Rooms",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpeningTime",
                table: "Rooms",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingTime",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "OpeningTime",
                table: "Rooms");
        }
    }
}
