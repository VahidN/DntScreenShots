namespace DntScreenShots.Services.Contracts;

public interface IExecuteConverterProcess
{
    Task<string> ExecuteChromeProcessAsync(
        string processName,
        string arguments,
        string converterExecutionPath,
        TimeSpan waitForExit);
}
