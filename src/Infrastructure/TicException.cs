namespace Tic.Console.Infrastructure;

public class TicException(string message) : Exception(message);

public class TicApiException(string message, int statusCode) : TicException(message)
{
    public int StatusCode { get; } = statusCode;
}
