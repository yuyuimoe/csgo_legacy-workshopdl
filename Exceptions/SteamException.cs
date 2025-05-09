namespace SupremacyWorkshopDownloader.Exceptions;

public class SteamException : Exception
{
    //Class responsible for base steam exceptions
    public SteamException(string message, Exception inner) : base(message, inner)
    {
    }
    public SteamException(string message) : base(message)
    {
    }
    public SteamException()
    {
    }
}