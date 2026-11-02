using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyRoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class LinkRoomsAndEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentRoom",
                columns: table => new
                {
                    EquipmentsId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentRoom", x => new { x.EquipmentsId, x.RoomsId });
                    table.ForeignKey(
                        name: "FK_EquipmentRoom_Equipments_EquipmentsId",
                        column: x => x.EquipmentsId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentRoom_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRoom_RoomsId",
                table: "EquipmentRoom",
                column: "RoomsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentRoom");
        }
    }
}
