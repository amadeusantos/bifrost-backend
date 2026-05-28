namespace Bifrost.Request;

public record CoordinationMemberBodyRequest(string role, Guid userId);