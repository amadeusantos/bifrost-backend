namespace Bifrost.Core.Exception.Coordination;

public class CoordinationMemberDuplicatedException()
    : CoreException(409, "Coordination members must not have duplicate users");
