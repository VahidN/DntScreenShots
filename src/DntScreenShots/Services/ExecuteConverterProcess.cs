using System.Text;
using DntScreenShots.Services.Contracts;

namespace DntScreenShots.Services;

public class ExecuteConverterProcess : IExecuteConverterProcess
{
    public async Task<string> ExecuteChromeProcessAsync(
        string processName,
        string arguments,
        string converterExecutionPath,
        TimeSpan waitForExit)
    {
        KillAllConverterProcesses(processName);

        var output = new StringBuilder();
        var waitToClose = TimeSpan.FromSeconds(5);

        using var process = new Process();
        process.StartInfo.FileName = converterExecutionPath;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.OutputDataReceived += OnProcessOnOutputDataReceived;
        process.ErrorDataReceived += OnProcessOnOutputDataReceived;
        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit((int)waitForExit.TotalMilliseconds);
        }
        finally
        {
            process.OutputDataReceived -= OnProcessOnOutputDataReceived;
            process.ErrorDataReceived -= OnProcessOnOutputDataReceived;
        }

        var errorMessage = output.ToString().Trim();

        var (exitCode, isExited) = GetProcessExitInfo(process);
        if (exitCode == 0 && isExited)
        {
            return errorMessage;
        }

        await Task.Delay(waitToClose);
        KillAllConverterProcesses(processName);
        throw new InvalidOperationException(Invariant($"HasExited: {isExited}, ExitCode: {exitCode}, {errorMessage}"));

        void OnProcessOnOutputDataReceived(object o, DataReceivedEventArgs e)
        {
            output.AppendLine(e.Data);
        }
    }

    private static void KillAllConverterProcesses(string processName)
    {
        var chromeProcesses = Process.GetProcessesByName(processName);
        foreach (var chromeProcess in chromeProcesses)
        {
            try
            {
                chromeProcess.Kill(true);
            }
            catch
            {
                // It's not important!
            }
        }
    }

    private static (int ExitCode, bool IsExited) GetProcessExitInfo(Process process)
    {
        try
        {
            return (process.ExitCode, process.HasExited);
        }
        catch
        {
            // It's not important!
        }

        return (-1, false);
    }
}
