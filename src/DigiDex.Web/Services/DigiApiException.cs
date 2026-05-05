namespace DigiDex.Web.Services;

public sealed class DigiApiException : Exception
{
    public DigiApiException(string message)
        : base(message)
    {
    }

    public DigiApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
