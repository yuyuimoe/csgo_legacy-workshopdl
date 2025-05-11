// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SupremacyWorkshopDownloader.SteamHandler;
using NativeFileDialogNET;

namespace SupremacyWorkshopDownloader;

internal class Program
{
    
    static string GetSupremacyDir()
    {
        using var selectFileDialog = new NativeFileDialog()
            .SelectFile()
            .AddFilter("Supremacy csgo.exe", "exe");
        
        DialogResult result = selectFileDialog.Open(out string[]? supremacyExecutable, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
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
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger<Program>();
        
        logger.LogInformation("Select CS Supremacy root folder");
        
        
        Constants.SupremacyDir = GetSupremacyDir();
        if (Constants.SupremacyDir == string.Empty)
            throw new Exception("Could not get supremacy directory");
        
        logger.LogInformation("Selected supremacy folder as {supremacyDir}", Constants.SupremacyDir);

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
