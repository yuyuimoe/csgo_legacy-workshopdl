using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SupremacyWorkshopDownloader.Exceptions;
using SupremacyWorkshopDownloader.SteamCmdHandler;

namespace SupremacyWorkshopDownloader.SteamHandler;

internal class SteamOutputInterpreter : IDisposable
{
    private readonly ILogger _logger;
    private readonly StreamWriter _sw;

    internal SteamOutputInterpreter()
    {
        _sw = new StreamWriter(Path.Combine(Constants.CurrentDir, "steamoutput.log"));
        _logger = LoggerFactory.Create(builder => 
            builder.AddConsole()
            .SetMinimumLevel(LogLevel.Debug))
            .CreateLogger("SteamOutputInterpreter");
    }
    
    internal async Task HandleOutput(string output)
    {
        if (Constants.RegexError.IsMatch(output))
        {
            this.Error(output);
        }
        
        if (Constants.RegexDownloadPath.IsMatch(output))
        {
            var swh = new SteamWorkshopHandler();
            string downloadPath = Constants.RegexDownloadPath.Match(output).Value;
            swh.ExtractWorkshopBinary(downloadPath);
            swh.DeleteFile(downloadPath);
        }

        await _sw.WriteLineAsync(output);
        //_logger.LogInformation("{output}", output);
        Console.WriteLine(output);
    }

    private void Error(string message)
    {
        throw new SteamException(message);
    }

    private void SteamUpdating(string message)
    {
        Console.Title = $"SteamCMD Updating... {message}";
    }
    
    public void Dispose()
    {
        _sw.Close();
        _sw.Dispose();
    }
}