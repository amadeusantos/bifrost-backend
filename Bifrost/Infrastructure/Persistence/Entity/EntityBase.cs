using System.ComponentModel.DataAnnotations;

namespace Bifrost.Infrastructure.Persistence.Entity;

public class EntityBase
{
    [Key]
    public Guid Id { get; set; }
}