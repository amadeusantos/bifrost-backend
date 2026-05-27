using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bifrost.Core.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Entity;

[Table("users")]
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(GoogleOpenid), IsUnique = true)]
public class UserEntity: EntityBase
{
    [MaxLength(128)]
    [Column("full_name")]
    public string? FullName { get; set; }
    [MaxLength(64)]
    [Column("email")]
    public required string Email { get; set; }
    [MaxLength(64)]
    [Column("google_openid")]
    public string? GoogleOpenid { get; set; }
    [MaxLength(32)]
    [Column("profile")]
    [DefaultValue(UserProfileEnum.Student)]
    public UserProfileEnum Profile { get; set; }
    [Column("is_admin")]
    [DefaultValue(false)]
    public bool IsAdmin { get; set; }
    [ForeignKey(nameof(CourseId))]
    [Column("course_id")]
    public Guid? CourseId { get; set; }

    public CourseEntity? Course { get; set; }
}