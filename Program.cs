// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SupremacyWorkshopDownloader.SteamHandler;

namespace SupremacyWorkshopDownloader;

internal class Program
{
    static async Task Main(string[] args)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger<Program>();

        SteamDownloader steamDownloader = new SteamDownloader(new HttpClient());
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = steamDownloader.GetPath(),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        SteamOutputInterpreter steamOutputInterpreter = new SteamOutputInterpreter();
        using CancellationTokenSource cts = new CancellationTokenSource();
        await using SteamRunner steamRunner = new SteamRunner(steamDownloader, startInfo, steamOutputInterpreter, cts);
        await steamRunner.DownloadWorkshopMap(308490450);
    }
}
