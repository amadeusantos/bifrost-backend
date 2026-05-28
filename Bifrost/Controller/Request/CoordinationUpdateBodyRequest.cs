using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record CoordinationUpdateBodyRequest(
    [Required] 
    [MaxLength(32)]
    string Name,
    [MinLength(1)]
    IEnumerable<CoordinationMemberBodyRequest> CoordinationMembers);