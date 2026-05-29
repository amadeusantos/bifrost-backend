namespace Bifrost.Core.Exception.Authentication;

public class InvalidOAuthCodeException() : CoreException(400, "Invalid or expired OAuth code");
