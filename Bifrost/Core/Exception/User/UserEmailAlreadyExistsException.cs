namespace Bifrost.Core.Exception.User;

public class UserEmailAlreadyExistsException() : CoreException(409, "User email already exists");