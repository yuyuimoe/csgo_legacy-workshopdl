using System.IO.Compression;

namespace SupremacyWorkshopDownloader.SteamCmdHandler;

public class SteamWorkshopHandler
{
    public void ExtractWorkshopBinary(string filePath)
    {
        if(!File.Exists(filePath))
            throw new FileNotFoundException("Workshop map not found", filePath);
        
        if(!filePath.EndsWith(".bin"))
            throw new InvalidDataException("File is not a binary file.");

        string workshopDir = Path.Combine(Constants.SupremacyDir, "csgo", "maps", "workshop");
        if(!Directory.Exists(workshopDir))
            Directory.CreateDirectory(workshopDir);
        
        string fullDir = Path.Combine(workshopDir, Path.GetFileName(Path.GetDirectoryName(filePath)) ?? "00000000");
        using ZipArchive zip = new ZipArchive(File.OpenRead(filePath), ZipArchiveMode.Read);
        zip.ExtractToDirectory(fullDir);
    }
    
    public void DeleteFile(string filePath)
    {
        if(File.Exists(filePath))
            File.Delete(filePath);
        
        if(Directory.Exists(Path.GetDirectoryName(filePath)))
            Directory.Delete(Path.GetDirectoryName(filePath), true);
    }
}