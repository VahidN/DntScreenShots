using System.Runtime.Versioning;
using DntScreenShots.Services.Contracts;

namespace DntScreenShots.Services;

/// <summary>
///     Defines the entry point of the application
/// </summary>
public class AppRunnerService : IAppRunnerService
{
    private readonly ICreateScreenshotsService _createScreenshotsService;

    public AppRunnerService(ICreateScreenshotsService createScreenshotsService) =>
        _createScreenshotsService = createScreenshotsService;

    [SupportedOSPlatform("Windows")]
    public Task StartAsync(string[] args) => _createScreenshotsService.StartAsync();
}
