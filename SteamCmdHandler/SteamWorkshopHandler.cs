using System.IO.Compression;

namespace SupremacyWorkshopDownloader.SteamCmdHandler;

public class SteamWorkshopHandler
{
    public void ExtractWorkshopMap(string filePath)
    {
        if(!File.Exists(filePath))
            throw new FileNotFoundException("Workshop map not found", filePath);
        
        using ZipArchive zip = new ZipArchive(File.OpenRead(filePath), ZipArchiveMode.Read);
        zip.ExtractToDirectory(Constants.BinaryDirPath);
    }
}