using System.Text.RegularExpressions;

namespace SupremacyWorkshopDownloader;

internal static class Constants
{
    internal const string BinaryDirName = "bin";
    internal static readonly string CurrentDir = Directory.GetCurrentDirectory();
    internal static readonly string BinaryDirPath = Path.Combine(CurrentDir, BinaryDirName);
    internal static readonly string SteamCmdPath = Path.Combine(CurrentDir, "workshop.*.bin");
    internal static readonly Regex RegexDownloadPath = new Regex(@"(?<=to\s.).*workshop.*_legacy\.bin");
    internal static readonly Regex RegexDownloading = new Regex(@"^Downloading item \d*");
    internal static readonly Regex RegexDownloadSuccess = new Regex(@"^Success. Downloaded item \d*");
    internal static readonly Regex RegexUpdateOutput = new Regex(@"^(\[\s*\d+%\]|\[-+\]).+$");
    internal static readonly Regex RegexTimeout = new Regex(@"\(Timeout\).+$");
    internal static readonly Regex RegexError = new Regex(@"^ERROR!\s.+$");
}