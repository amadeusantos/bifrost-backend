using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bifrost.Migrations
{
    /// <inheritdoc />
    public partial class CreateAcademicCenterAndAcademicCenterMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "academic_centers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    avg_score = table.Column<decimal>(type: "numeric(8,6)", nullable: true),
                    assessment_season_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_academic_centers", x => x.id);
                    table.ForeignKey(
                        name: "FK_academic_centers_assessment_seasons_assessment_season_id",
                        column: x => x.assessment_season_id,
                        principalTable: "assessment_seasons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "academic_center_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    academic_center_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_academic_center_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_academic_center_members_academic_centers_academic_center_id",
                        column: x => x.academic_center_id,
                        principalTable: "academic_centers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_academic_center_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_academic_center_members_academic_center_id",
                table: "academic_center_members",
                column: "academic_center_id");

            migrationBuilder.CreateIndex(
                name: "IX_academic_center_members_user_id_academic_center_id",
                table: "academic_center_members",
                columns: new[] { "user_id", "academic_center_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_academic_centers_assessment_season_id",
                table: "academic_centers",
                column: "assessment_season_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "academic_center_members");

            migrationBuilder.DropTable(
                name: "academic_centers");
        }
    }
}
