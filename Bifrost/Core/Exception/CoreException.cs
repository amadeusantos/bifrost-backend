namespace Bifrost.Core.Exception;

public class CoreException(int statusCode, string message) : System.Exception(message)
{
    public int StatusCode { get; set; } = statusCode;
};