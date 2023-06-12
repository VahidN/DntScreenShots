using DntScreenShots.Models;
using DntScreenShots.Services.Contracts;

namespace DntScreenShots.Tests.Factory;

public class FakeFetchDownloadItemsService : IFetchDownloadItemsService
{
    public Task<List<DownloadItem>?> GetDownloadItemsAsync() =>
        Task.FromResult<List<DownloadItem>?>(new List<DownloadItem>
                                             {
                                                 new()
                                                 {
                                                     Id = 1,
                                                     Url =
                                                         "https://www.youtube.com/playlist?list=PLaxGk79fk9i3oxzWhlefIuUNbtNqIXjkI",
                                                 },
                                                 new()
                                                 {
                                                     Id = 2,
                                                     Url =
                                                         "https://betterprogramming.pub/all-javascript-and-typescript-features-of-the-last-3-years-629c57e73e42",
                                                 },
                                                 new() { Id = 3, Url = "https://www.dntips.ir" },
                                             });
}
