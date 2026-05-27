using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Entity;

[Table("assessment_seasons")]
[Index(nameof(Period), IsUnique = true)]
public class AssessmentSeasonEntity: EntityBase
{

    [Column("period", TypeName = "varchar(7)")]
    public required string Period { get; set; }

    [Column("start_date_time")] public DateTime? StartDateTime { get; set; }

    [Column("end_date_time")] public DateTime? EndDateTime { get; set; }
    
    [ForeignKey(nameof(CourseId))]
    [Column("course_id")] public required Guid CourseId { get; set; }

    public CourseEntity Course { get; set; } = null!;
}
