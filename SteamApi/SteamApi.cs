namespace SupremacyWorkshopDownloader.SteamApi;

internal class SteamApi(SteamApiRequests requests)
{
    public async Task<string> getWorkshopMap(int workshopId)
    {
        //TODO: Return an object that has GetPublishedFileDetails properties.
        return await requests.GetPublishedFileDetails(workshopId);
    }
}