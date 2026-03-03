using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FoodLovera.Infrastructure.Migrations
{
    public partial class FixBannedEmailBannedByType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannedEmails");

            migrationBuilder.CreateTable(
                name: "BannedEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),

                    EmailNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),

                    BannedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),

                    BannedByUserId = table.Column<int>(type: "integer", nullable: true),

                    Reason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannedEmails", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BannedEmails_EmailNormalized",
                table: "BannedEmails",
                column: "EmailNormalized",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropTable(
                name: "BannedEmails");
        }
    }
}