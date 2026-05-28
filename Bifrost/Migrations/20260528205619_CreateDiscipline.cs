using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bifrost.Migrations
{
    /// <inheritdoc />
    public partial class CreateDiscipline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "disciplines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    avg_score = table.Column<decimal>(type: "numeric(8,6)", nullable: true),
                    assessment_season_id = table.Column<Guid>(type: "uuid", nullable: false),
                    professor_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disciplines", x => x.id);
                    table.ForeignKey(
                        name: "FK_disciplines_assessment_seasons_assessment_season_id",
                        column: x => x.assessment_season_id,
                        principalTable: "assessment_seasons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_disciplines_users_professor_id",
                        column: x => x.professor_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "discipline_students",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    discipline_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discipline_students", x => x.id);
                    table.ForeignKey(
                        name: "FK_discipline_students_disciplines_discipline_id",
                        column: x => x.discipline_id,
                        principalTable: "disciplines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_discipline_students_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_discipline_students_discipline_id_user_id",
                table: "discipline_students",
                columns: new[] { "discipline_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_discipline_students_user_id",
                table: "discipline_students",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_disciplines_assessment_season_id",
                table: "disciplines",
                column: "assessment_season_id");

            migrationBuilder.CreateIndex(
                name: "IX_disciplines_professor_id",
                table: "disciplines",
                column: "professor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "discipline_students");

            migrationBuilder.DropTable(
                name: "disciplines");
        }
    }
}
