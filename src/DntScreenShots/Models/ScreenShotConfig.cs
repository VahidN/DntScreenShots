namespace DntScreenShots.Models;

public class ScreenShotConfig
{
    public int ScreenshotWidth { set; get; }
    public int ScreenshotHeight { set; get; }
    public int BadScreenshotLength { set; get; }
    public required string ScreenshotsFolderName { set; get; }
    public required string TempOutputPngFileName { set; get; }
    public required string TempOutputCompressedPngFileName { set; get; }
    public required string ScreenshotsListFileName { set; get; }
    public TimeSpan DelayBetweenActions { set; get; }
    public TimeSpan WaitForExit { set; get; }
    public required string IncompleteDownloadItemsFileName { set; get; }
    public int AllowedRetries { set; get; }
}
