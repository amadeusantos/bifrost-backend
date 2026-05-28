using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Entity;

[Table("academic_centers")]
[Index(nameof(AssessmentSeasonId), IsUnique = true)]
public class AcademicCenterEntity : EntityBase
{
    [MaxLength(32)]
    [Column("name")]
    public required string Name { get; set; }

    [Column("avg_score", TypeName = "decimal(8,6)")]
    public decimal? AvgScore { get; set; }

    [ForeignKey(nameof(AssessmentSeasonId))]
    [Column("assessment_season_id")]
    public required Guid AssessmentSeasonId { get; set; }

    public AssessmentSeasonEntity AssessmentSeason { get; set; } = null!;
    public required List<AcademicCenterMemberEntity> AcademicCenterMembers { get; set; }
}
