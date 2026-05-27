using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Entity;

[Table("courses")]
[Index(nameof(Code), IsUnique = true)]
public class CourseEntity: EntityBase
{
    [MaxLength(64)]
    public required string Name { get; set; } = null!;
    
    [MaxLength(32)]
    public required string Code { get; set; } = null!;
}