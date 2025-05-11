using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace SupremacyWorkshopDownloader.SteamCmdHandler;

public class SteamWorkshopHandler
{
    public void ExtractWorkshopBinary(string filePath)
    {
        if(!File.Exists(filePath))
            throw new FileNotFoundException("Workshop map not found", filePath);
        
        if(!filePath.EndsWith(".bin"))
            throw new InvalidDataException("File is not a binary file.");
        
        Console.WriteLine("Extracting map...");
        string workshopDir = Path.Combine(Constants.SupremacyDir, "csgo", "maps", "workshop");
        if (!Directory.Exists(workshopDir))
        {
            Program.Logger.LogDebug("Creating workshop directory");
            Directory.CreateDirectory(workshopDir);
        }
        
        string fullDir = Path.Combine(workshopDir, Path.GetFileName(Path.GetDirectoryName(filePath)) ?? "00000000");
        using ZipArchive zip = new ZipArchive(File.OpenRead(filePath), ZipArchiveMode.Read);
        zip.ExtractToDirectory(fullDir);
        Console.WriteLine("Map extracted to {0}", fullDir);
    }
    
    public void DeleteFile(string filePath)
    {
        if(File.Exists(filePath))
            File.Delete(filePath);
        
        if(Directory.Exists(Path.GetDirectoryName(filePath)))
            Directory.Delete(Path.GetDirectoryName(filePath), true);
    }
}