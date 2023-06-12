using DntScreenShots.Models;
using DntScreenShots.Services.Contracts;
using Microsoft.Extensions.Options;

namespace DntScreenShots.Services;

public class FetchDownloadItemsService : IFetchDownloadItemsService
{
    private readonly IOptions<AppConfig> _appConfig;
    private readonly AppHttpClientService _appHttpClientService;

    public FetchDownloadItemsService(AppHttpClientService appHttpClientService,
                                     IOptions<AppConfig> appConfig)
    {
        _appHttpClientService = appHttpClientService;
        _appConfig = appConfig;
    }

    public async Task<List<DownloadItem>?> GetDownloadItemsAsync()
    {
        var jsonList =
            await _appHttpClientService.DownloadPageAsync(_appConfig.Value.HttpClientConfig.DownloadsListUrl);
        var downloadItems = JsonSerializer.Deserialize<List<DownloadItem>>(jsonList,
                                                                           new JsonSerializerOptions
                                                                           {
                                                                               PropertyNameCaseInsensitive = true,
                                                                           });
        return downloadItems;
    }
}
