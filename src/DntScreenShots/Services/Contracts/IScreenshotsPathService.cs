namespace DntScreenShots.Services.Contracts;

public interface IScreenshotsPathService
{
    string Root { get; }
    string ScreenshotsFolderPath { get; }
    string ScreenshotsListFilePath { get; }
    string TempOutputPngFilePath { get; }
    string TempOutputCompressedPngFilePath { get; }
    string IncompleteDownloadItemsPath { get; }
}
