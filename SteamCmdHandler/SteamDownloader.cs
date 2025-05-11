using System.Formats.Tar;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace SupremacyWorkshopDownloader.SteamHandler;

internal class SteamDownloader
{
    private HttpClient _client;
    internal SteamDownloader(HttpClient client)
    {
        _client = client;
    }
    
    internal async Task<bool> Download()
    {
        if (IsSteamCmdExist())
            return true;
        
        Console.WriteLine("Downloading SteamCMD...");
        
        if (!Directory.Exists(Constants.BinaryDirPath))
            Directory.CreateDirectory(Constants.BinaryDirPath);
        
        await using Stream stream = await _client.GetStreamAsync(GetDownloadUrl());
        await ExtractDownload(stream);
        return IsSteamCmdExist();
    }

    private async Task ExtractDownload(Stream stream)
    {
        Console.WriteLine("Extracting SteamCMD...");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
            zip.ExtractToDirectory(Constants.SteamCmdPath);
        }
        GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress);
        await TarFile.ExtractToDirectoryAsync(gzip, Constants.SteamCmdPath, true, CancellationToken.None);
    }

    private bool IsSteamCmdExist()
    {
        return File.Exists(GetPath());
    }

    internal string GetPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(Constants.BinaryDirPath, "steamcmd.exe");
        }

        return Path.Combine(Constants.BinaryDirPath, "steamcmd.sh");
    }
    
    private string GetDownloadUrl()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
        }

        return "https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz";
    }
}