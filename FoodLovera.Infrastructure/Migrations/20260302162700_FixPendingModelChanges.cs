using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodLovera.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentRestaurantId",
                table: "Sessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentRestaurantId",
                table: "Sessions",
                type: "uuid",
                nullable: true);
        }
    }
}
