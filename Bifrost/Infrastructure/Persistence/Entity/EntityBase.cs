using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bifrost.Infrastructure.Persistence.Entity;

public class EntityBase
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
}