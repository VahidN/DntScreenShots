using DntScreenShots.Models;
using Microsoft.Extensions.Options;

namespace DntScreenShots.Services.Contracts;

public class IncompleteDownloadItemsService : IIncompleteDownloadItemsService
{
    private readonly IOptions<AppConfig> _appConfig;
    private readonly IScreenshotsPathService _screenshotsPathService;
    private List<IncompleteDownloadItem> _incompleteDownloadItems = new();

    public IncompleteDownloadItemsService(IScreenshotsPathService screenshotsPathService,
                                          IOptions<AppConfig> appConfig)
    {
        _screenshotsPathService = screenshotsPathService;
        _appConfig = appConfig;
        LoadIncompleteDownloadItems();
    }

    public void AddIncompleteDownloadItem(DownloadItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        var existingItem = FindItem(item);
        if (existingItem is null)
        {
            _incompleteDownloadItems.Add(new IncompleteDownloadItem { Id = item.Id, Url = item.Url, Retries = 1 });
        }
        else
        {
            existingItem.Retries++;
        }
    }

    public void SaveChanges()
    {
        var path = _screenshotsPathService.IncompleteDownloadItemsPath;
        var json = JsonSerializer.Serialize(_incompleteDownloadItems,
                                            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public bool ShouldProcessItem(DownloadItem item)
    {
        var existingItem = FindItem(item);
        if (existingItem is null)
        {
            return true;
        }

        return existingItem.Retries <= _appConfig.Value.ScreenShotConfig.AllowedRetries;
    }

    private IncompleteDownloadItem? FindItem(DownloadItem item)
    {
        return _incompleteDownloadItems.Find(downloadItem => downloadItem.Id == item.Id);
    }

    private void LoadIncompleteDownloadItems()
    {
        var path = _screenshotsPathService.IncompleteDownloadItemsPath;
        if (!File.Exists(path))
        {
            return;
        }

        var jsonList = File.ReadAllText(path);
        _incompleteDownloadItems = JsonSerializer.Deserialize<List<IncompleteDownloadItem>>(jsonList,
                                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
                                   new List<IncompleteDownloadItem>();
    }
}
