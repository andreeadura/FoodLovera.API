using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodLovera.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionOutcomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Cities_CityId",
                table: "Restaurants");

            migrationBuilder.CreateTable(
                name: "SessionOutcomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionOutcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionOutcomes_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionOutcomeRestaurants",
                columns: table => new
                {
                    OutcomeId = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LikeCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionOutcomeRestaurants", x => new { x.OutcomeId, x.RestaurantId });
                    table.ForeignKey(
                        name: "FK_SessionOutcomeRestaurants_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionOutcomeRestaurants_SessionOutcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "SessionOutcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantRestaurantActions_ParticipantId",
                table: "ParticipantRestaurantActions",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantRestaurantActions_RestaurantId",
                table: "ParticipantRestaurantActions",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionOutcomeRestaurants_RestaurantId",
                table: "SessionOutcomeRestaurants",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionOutcomes_SessionId",
                table: "SessionOutcomes",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantRestaurantActions_Restaurants_RestaurantId",
                table: "ParticipantRestaurantActions",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantRestaurantActions_SessionParticipants_Participan~",
                table: "ParticipantRestaurantActions",
                column: "ParticipantId",
                principalTable: "SessionParticipants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantRestaurantActions_Sessions_SessionId",
                table: "ParticipantRestaurantActions",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Cities_CityId",
                table: "Restaurants",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantRestaurantActions_Restaurants_RestaurantId",
                table: "ParticipantRestaurantActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantRestaurantActions_SessionParticipants_Participan~",
                table: "ParticipantRestaurantActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantRestaurantActions_Sessions_SessionId",
                table: "ParticipantRestaurantActions");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Cities_CityId",
                table: "Restaurants");

            migrationBuilder.DropTable(
                name: "SessionOutcomeRestaurants");

            migrationBuilder.DropTable(
                name: "SessionOutcomes");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantRestaurantActions_ParticipantId",
                table: "ParticipantRestaurantActions");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantRestaurantActions_RestaurantId",
                table: "ParticipantRestaurantActions");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Cities_CityId",
                table: "Restaurants",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
