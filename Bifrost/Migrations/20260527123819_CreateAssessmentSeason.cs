using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bifrost.Migrations
{
    /// <inheritdoc />
    public partial class CreateAssessmentSeason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "assessment_seasons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    period = table.Column<string>(type: "varchar(7)", nullable: false),
                    start_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assessment_seasons", x => x.id);
                    table.ForeignKey(
                        name: "FK_assessment_seasons_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assessment_seasons_course_id",
                table: "assessment_seasons",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_assessment_seasons_period",
                table: "assessment_seasons",
                column: "period",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assessment_seasons");
        }
    }
}
