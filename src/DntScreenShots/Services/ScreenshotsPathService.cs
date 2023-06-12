using DntScreenShots.Models;
using DntScreenShots.Services.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DntScreenShots.Services;

public class ScreenshotsPathService : IScreenshotsPathService
{
    public ScreenshotsPathService(IHostEnvironment hostEnvironment,
                                  IOptions<AppConfig> appConfig,
                                  ILogger<ScreenshotsPathService> logger)
    {
        var screenShotConfig = appConfig.Value.ScreenShotConfig;
        Root = GetRootPath(hostEnvironment);
        logger.LogInformation("Using root: {Root}", Root);

        ScreenshotsFolderPath = Path.Combine(Root, screenShotConfig.ScreenshotsFolderName);
        CheckDirExists(ScreenshotsFolderPath);
        ScreenshotsListFilePath = Path.Combine(ScreenshotsFolderPath, screenShotConfig.ScreenshotsListFileName);
        TempOutputPngFilePath = Path.Combine(ScreenshotsFolderPath, screenShotConfig.TempOutputPngFileName);
        TempOutputCompressedPngFilePath = Path.Combine(ScreenshotsFolderPath,
                                                       screenShotConfig.TempOutputCompressedPngFileName);
        IncompleteDownloadItemsPath = Path.Combine(ScreenshotsFolderPath,
                                                   screenShotConfig.IncompleteDownloadItemsFileName);
    }

    public string IncompleteDownloadItemsPath { get; }

    public string Root { get; }
    public string ScreenshotsFolderPath { get; }
    public string ScreenshotsListFilePath { get; }
    public string TempOutputPngFilePath { get; }
    public string TempOutputCompressedPngFilePath { get; }

    private static string GetRootPath(IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.ContentRootPath.Split(new[]
                                                     {
                                                         "\\bin\\",
                                                     },
                                                     StringSplitOptions.RemoveEmptyEntries)[0];
    }

    private static void CheckDirExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
