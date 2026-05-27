namespace Bifrost.Core.Exception.User;

public class UserNotFoundException() : CoreException(404, "User not found");