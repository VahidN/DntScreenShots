using DntScreenShots.Models;
using DntScreenShots.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DntScreenShots.Services;

public static class ServicesRegistry
{
    public static void AddServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.TryAddSingleton<IIncompleteDownloadItemsService, IncompleteDownloadItemsService>();
        serviceCollection.TryAddSingleton<IFetchDownloadItemsService, FetchDownloadItemsService>();
        serviceCollection.TryAddSingleton<IHtmlToPngGenerator, ChromeHtmlToPngGenerator>();
        serviceCollection.TryAddSingleton<IScreenshotsPathService, ScreenshotsPathService>();
        serviceCollection.TryAddSingleton<IExecuteConverterProcess, ExecuteConverterProcess>();
        serviceCollection.TryAddSingleton<ICreateScreenshotsService, CreateScreenshotsService>();
        serviceCollection.TryAddSingleton<IAppRunnerService, AppRunnerService>();
        serviceCollection.Configure<AppConfig>(configuration.Bind);
        serviceCollection.AddHttpClient(configuration);
    }
}
