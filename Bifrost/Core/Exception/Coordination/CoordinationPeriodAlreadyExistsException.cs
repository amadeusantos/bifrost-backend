namespace Bifrost.Core.Exception.Coordination;

public class CoordinationPeriodAlreadyExistsException()
    : CoreException(409, "Coordination period already exists");