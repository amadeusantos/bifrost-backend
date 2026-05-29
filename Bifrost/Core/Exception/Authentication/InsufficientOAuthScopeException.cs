namespace Bifrost.Core.Exception.Authentication;

public class InsufficientOAuthScopeException() : CoreException(403, "OAuth scope is invalid or incomplete");
