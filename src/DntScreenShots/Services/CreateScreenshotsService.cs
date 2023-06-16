using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using DntScreenShots.Models;
using DntScreenShots.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PnnQuant;

namespace DntScreenShots.Services;

public class CreateScreenshotsService : ICreateScreenshotsService
{
    private readonly IOptions<AppConfig> _appConfig;
    private readonly IFetchDownloadItemsService _fetchDownloadItemsService;
    private readonly IHtmlToPngGenerator _htmlToPngGenerator;
    private readonly IIncompleteDownloadItemsService _incompleteDownloadItemsService;
    private readonly ILogger<CreateScreenshotsService> _logger;
    private readonly IScreenshotsPathService _screenshotsPathService;

    public CreateScreenshotsService(ILogger<CreateScreenshotsService> logger,
                                    IOptions<AppConfig> appConfig,
                                    IHtmlToPngGenerator htmlToPngGenerator,
                                    IFetchDownloadItemsService fetchDownloadItemsService,
                                    IScreenshotsPathService screenshotsPathService,
                                    IIncompleteDownloadItemsService incompleteDownloadItemsService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        _htmlToPngGenerator = htmlToPngGenerator;
        _fetchDownloadItemsService = fetchDownloadItemsService;
        _screenshotsPathService = screenshotsPathService;
        _incompleteDownloadItemsService = incompleteDownloadItemsService;
    }

    [SupportedOSPlatform("Windows")]
    public async Task StartAsync()
    {
        try
        {
            var downloadItems = await _fetchDownloadItemsService.GetDownloadItemsAsync();
            if (downloadItems is not { Count: not 0 })
            {
                _logger.LogInformation("There is nothing to process.");
                return;
            }

            var screenShotConfig = _appConfig.Value.ScreenShotConfig;
            var downloadsCount = downloadItems.Count;
            var downloadIndex = 0;

            foreach (var downloadItem in downloadItems)
            {
                downloadIndex++;

                if (string.IsNullOrWhiteSpace(downloadItem.Url))
                {
                    continue;
                }

                var destFileName = Path.Combine(_screenshotsPathService.ScreenshotsFolderPath,
                                                Invariant($"news-{downloadItem.Id}.jpg"));
                if (File.Exists(destFileName))
                {
                    _logger.LogInformation("Skipping an already processed URL -> `{Url}`", downloadItem.Url);
                    continue;
                }

                if (!_incompleteDownloadItemsService.ShouldProcessItem(downloadItem))
                {
                    _logger.LogInformation("Skipping an already processed and faulty URL -> `{Url}`", downloadItem.Url);

                    File.Copy(_screenshotsPathService.NullImageFilePath, destFileName);
                    _logger.LogInformation("`{DestFileName}` has been added.", destFileName);

                    continue;
                }

                try
                {
                    _logger.LogInformation("Processing[{DownloadIndex}/{DownloadsCount}] {Url}",
                                           downloadIndex,
                                           downloadsCount,
                                           downloadItem.Url);
                    var result = await _htmlToPngGenerator.GeneratePngFromHtmlAsync(
                                  new HtmlToPngGeneratorOptions
                                  {
                                      SourceHtmlFileOrUri = downloadItem.Url,
                                      OutputPngFile = _screenshotsPathService.TempOutputPngFilePath,
                                      ConverterExecutionPath = "",
                                      WaitForExit = screenShotConfig.WaitForExit,
                                      Width = screenShotConfig.ScreenshotWidth,
                                      Height = screenShotConfig.ScreenshotHeight,
                                  });
                    _logger.LogInformation("{Result}", result);

                    await Task.Delay(screenShotConfig.DelayBetweenActions);

                    try
                    {
                        CompressPngFile(_screenshotsPathService.TempOutputPngFilePath,
                                        _screenshotsPathService.TempOutputCompressedPngFilePath);
                        if (new FileInfo(_screenshotsPathService.TempOutputCompressedPngFilePath).Length <
                            screenShotConfig.BadScreenshotLength)
                        {
                            _incompleteDownloadItemsService.AddIncompleteDownloadItem(downloadItem);
                            _logger.LogCritical("Couldn't process {Url}", downloadItem.Url);
                            continue;
                        }

                        File.Move(_screenshotsPathService.TempOutputCompressedPngFilePath, destFileName);
                        _logger.LogInformation("`{DestFileName}` has been added.", destFileName);
                    }
                    catch (Exception ex)
                    {
                        _incompleteDownloadItemsService.AddIncompleteDownloadItem(downloadItem);
                        _logger.LogCritical(ex, "Couldn't process {Url}", downloadItem.Url);
                        File.Move(_screenshotsPathService.TempOutputPngFilePath, destFileName);
                    }
                }
                catch (Exception ex)
                {
                    _incompleteDownloadItemsService.AddIncompleteDownloadItem(downloadItem);
                    _logger.LogCritical(ex, "Couldn't process {Url}", downloadItem.Url);
                }

                await Task.Delay(screenShotConfig.DelayBetweenActions);
            }

            CreateScreenShotsList(_screenshotsPathService.ScreenshotsListFilePath,
                                  _screenshotsPathService.ScreenshotsFolderPath);
            _incompleteDownloadItemsService.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error");
        }
    }

    private static void CreateScreenShotsList(string screenshotsListFilePath, string screenshotsFolderPath)
    {
        var filesList = string.Join(Environment.NewLine,
                                    new DirectoryInfo(screenshotsFolderPath).GetFiles("*.jpg")
                                                                            .Select(fileInfo => fileInfo.Name));
        File.WriteAllText(screenshotsListFilePath, filesList);
    }

    [SupportedOSPlatform("Windows")]
    private static void CompressPngFile(string sourcePath, string targetPath)
    {
        using var bitmap = new Bitmap(sourcePath);
        using var dest = new PnnQuantizer().QuantizeImage(bitmap, PixelFormat.Undefined, 128, true);
        dest.Save(targetPath, ImageFormat.Png);
    }
}
