using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Entity;

[Table("discipline_students")]
[Index(nameof(DisciplineId), nameof(UserId), IsUnique = true)]
public class DisciplineStudentEntity : EntityBase
{
    [ForeignKey(nameof(DisciplineId))]
    [Column("discipline_id")]
    public required Guid DisciplineId { get; set; }

    public DisciplineEntity Discipline { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [Column("user_id")]
    public required Guid UserId { get; set; }

    public UserEntity User { get; set; } = null!;
}
