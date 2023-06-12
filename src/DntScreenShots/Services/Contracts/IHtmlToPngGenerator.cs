using DntScreenShots.Models;

namespace DntScreenShots.Services.Contracts;

public interface IHtmlToPngGenerator
{
    /// <summary>
    ///     High level method that converts HTML to PNG.
    /// </summary>
    Task<string> GeneratePngFromHtmlAsync(HtmlToPngGeneratorOptions options);
}
