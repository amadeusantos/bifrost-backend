using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bifrost.Infrastructure.Persistence.Entity;

[Table("disciplines")]
public class DisciplineEntity : EntityBase
{
    [MaxLength(32)]
    [Column("name")]
    public required string Name { get; set; }

    [MaxLength(32)]
    [Column("code")]
    public required string Code { get; set; }

    [Column("avg_score", TypeName = "decimal(8,6)")]
    public decimal? AvgScore { get; set; }

    [ForeignKey(nameof(AssessmentSeasonId))]
    [Column("assessment_season_id")]
    public required Guid AssessmentSeasonId { get; set; }

    public AssessmentSeasonEntity AssessmentSeason { get; set; } = null!;

    [ForeignKey(nameof(ProfessorId))]
    [Column("professor_id")]
    public required Guid ProfessorId { get; set; }

    public UserEntity Professor { get; set; } = null!;

    public List<DisciplineStudentEntity> DisciplineStudents { get; set; } = [];
}
