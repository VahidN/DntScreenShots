using DntScreenShots.Models;

namespace DntScreenShots.Services;

public interface IIncompleteDownloadItemsService
{
    void AddIncompleteDownloadItem(DownloadItem item);
    void SaveChanges();
    bool ShouldProcessItem(DownloadItem item);
}
