namespace DntScreenShots.Models;

public class HtmlToPngGeneratorOptions
{
    public required string SourceHtmlFileOrUri { set; get; }

    public required string OutputPngFile { set; get; }

    public required string ConverterExecutionPath { get; set; }

    public int Width { set; get; }

    public int Height { set; get; }

    public TimeSpan WaitForExit { set; get; }
}
