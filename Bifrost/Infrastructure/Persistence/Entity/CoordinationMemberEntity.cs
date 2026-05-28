using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Entity;

[Table("coordination_members")]
[Index(nameof(UserId), nameof(CoordinationId), IsUnique = true)]
public class CoordinationMemberEntity: EntityBase
{
    [MaxLength(32)]
    [Column("role")]
    public required string Role { get; set; }
    [ForeignKey(nameof(CoordinationId))]
    [Column("coordination_id")] 
    public required Guid CoordinationId { get; set; }
    public CoordinationEntity Coordination { get; set; } = null!;
    [ForeignKey(nameof(UserId))]
    [Column("user_id")] 
    public required Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}