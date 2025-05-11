// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SupremacyWorkshopDownloader.SteamHandler;
using NativeFileDialogNET;

namespace SupremacyWorkshopDownloader;

internal class Program
{
    public static ILogger Logger { get; private set; }
    static string GetSupremacyDir()
    {
        Logger.LogDebug("Opening file dialog");
        using var selectFileDialog = new NativeFileDialog()
            .SelectFile()
            .AddFilter("Supremacy csgo.exe", "exe");
        
        DialogResult result = selectFileDialog.Open(out string[]? supremacyExecutable, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        Logger.LogDebug("File dialog result: {result}", result);
        if (result == DialogResult.Cancel)
        {
            Console.WriteLine("Cancelled");
            return string.Empty;
        }
        
        if(result != DialogResult.Okay || supremacyExecutable == null)
        {
            Console.WriteLine("Failed to select Supremacy folder. Try Again");
            return GetSupremacyDir();
        }

        string? supremacyDir = Path.GetDirectoryName(supremacyExecutable[0]);
        return supremacyDir ?? String.Empty;
    }
    
    static async Task Main(string[] args)
    {
        LogLevel logLevel = LogLevel.Information;
        
        #if DEBUG
        logLevel = LogLevel.Debug;
        #endif
        
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(logLevel));
        Logger = factory.CreateLogger<Program>();
        
        Console.WriteLine("Select CS Supremacy csgo.exe");
        Constants.SupremacyDir = GetSupremacyDir();
        if (Constants.SupremacyDir == string.Empty)
            throw new Exception("Could not get supremacy directory");
        
        Console.WriteLine("Selected supremacy folder as {0}", Constants.SupremacyDir);
        Logger.LogDebug("Setting up SteamCMD");
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
        await steamRunner.DownloadWorkshopMap(298360922);
    }
}
