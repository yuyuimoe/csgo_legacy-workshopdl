using System.Text;
using System.Text.Json.Serialization;

namespace SupremacyWorkshopDownloader.SteamApi;

internal class SteamApiRequests(HttpClient httpClient)
{
    private static readonly string SteamApiUrl = "https://api.steampowered.com";
    private static readonly string RemoteStorageEndpoint = "/ISteamRemoteStorage";

    public async Task<string> GetPublishedFileDetails(int publishedFileId)
    {
        HttpContent content = new StringContent(
            $"{{\"itemcount\":1,\"publishedfileids\":[{publishedFileId}]}}",
            Encoding.UTF8,
            "application/json"
        );
        
        var response = await httpClient.PostAsync(
            SteamApiUrl + RemoteStorageEndpoint + "/GetPublishedFileDetails/v1/",
            content
        );
        return response.Content.ToString() ?? string.Empty;
    }
}