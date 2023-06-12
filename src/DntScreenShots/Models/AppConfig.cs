namespace DntScreenShots.Models;

public class AppConfig
{
    public required HttpClientConfig HttpClientConfig { set; get; }

    public required ScreenShotConfig ScreenShotConfig { set; get; }
}
