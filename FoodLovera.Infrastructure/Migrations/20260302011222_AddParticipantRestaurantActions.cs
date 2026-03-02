using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodLovera.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParticipantRestaurantActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParticipantRestaurantActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantRestaurantActions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantRestaurantActions_SessionId_ParticipantId_Restau~",
                table: "ParticipantRestaurantActions",
                columns: new[] { "SessionId", "ParticipantId", "RestaurantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipantRestaurantActions");
        }
    }
}
