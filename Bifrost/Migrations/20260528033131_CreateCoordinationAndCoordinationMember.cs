using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bifrost.Migrations
{
    /// <inheritdoc />
    public partial class CreateCoordinationAndCoordinationMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "coordinations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    avg_score = table.Column<decimal>(type: "numeric(8,6)", nullable: true),
                    assessment_season_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coordinations", x => x.id);
                    table.ForeignKey(
                        name: "FK_coordinations_assessment_seasons_assessment_season_id",
                        column: x => x.assessment_season_id,
                        principalTable: "assessment_seasons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "coordination_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    coordination_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coordination_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_coordination_members_coordinations_coordination_id",
                        column: x => x.coordination_id,
                        principalTable: "coordinations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_coordination_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_coordination_members_coordination_id",
                table: "coordination_members",
                column: "coordination_id");

            migrationBuilder.CreateIndex(
                name: "IX_coordination_members_user_id_coordination_id",
                table: "coordination_members",
                columns: new[] { "user_id", "coordination_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_coordinations_assessment_season_id",
                table: "coordinations",
                column: "assessment_season_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coordination_members");

            migrationBuilder.DropTable(
                name: "coordinations");
        }
    }
}
