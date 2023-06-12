using DntScreenShots.Models;

namespace DntScreenShots.Services.Contracts;

public interface IFetchDownloadItemsService
{
    Task<List<DownloadItem>?> GetDownloadItemsAsync();
}
