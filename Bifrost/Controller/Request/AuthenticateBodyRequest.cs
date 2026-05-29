using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record AuthenticateBodyRequest([Required] string Code);