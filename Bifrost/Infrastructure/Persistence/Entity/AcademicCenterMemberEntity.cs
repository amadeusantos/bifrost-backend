using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Entity;

[Table("academic_center_members")]
[Index(nameof(UserId), nameof(AcademicCenterId), IsUnique = true)]
public class AcademicCenterMemberEntity : EntityBase
{
    [MaxLength(32)]
    [Column("role")]
    public required string Role { get; set; }

    [ForeignKey(nameof(AcademicCenterId))]
    [Column("academic_center_id")]
    public required Guid AcademicCenterId { get; set; }

    public AcademicCenterEntity AcademicCenter { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [Column("user_id")]
    public required Guid UserId { get; set; }

    public UserEntity User { get; set; } = null!;
}
