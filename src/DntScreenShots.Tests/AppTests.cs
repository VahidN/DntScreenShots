using DntScreenShots.Services.Contracts;
using DntScreenShots.Tests.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DntScreenShots.Tests;

[TestClass]
public class AppTests
{
    [TestMethod]
    public async Task TestAppRunnerServiceWorks()
    {
        var screenshotsPathService = TestsAppFactory.GetRequiredService<IScreenshotsPathService>();
        var appRunnerService = TestsAppFactory.GetRequiredService<IAppRunnerService>();
        await appRunnerService.StartAsync(new[]
                                          {
                                              "",
                                          });
        Assert.IsTrue(File.Exists(screenshotsPathService.ScreenshotsListFilePath));
    }
}
