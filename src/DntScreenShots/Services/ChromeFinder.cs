using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace DntScreenShots.Services;

public static class ChromeFinder
{
    public static string Find()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new NotSupportedException("Please specify the ChromeExecutionPath.");
        }

        var key =
            Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe",
                              "Path", string.Empty);

        var chromeExe = "chrome.exe";
        if (key != null)
        {
            var path = key.ToString();
            if (path != null)
            {
                path = Path.Combine(path, chromeExe);
                if (File.Exists(path))
                {
                    return path;
                }
            }
        }

        var exeNames = new List<string> { chromeExe };

        var currentPath = GetAppPath();
        foreach (var path in exeNames.Select(exeName => Path.Combine(currentPath, exeName)))
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        var directories = new List<string>();
        GetApplicationDirectories(directories);

        foreach (var exeName in exeNames)
        {
            foreach (var path in directories.Select(directory => Path.Combine(directory, exeName)))
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
        }

        throw new NotSupportedException("Please specify the ChromeExecutionPath.");
    }

    private static void GetApplicationDirectories(ICollection<string> directories)
    {
        const string subDirectory = "Google\\Chrome\\Application";
        directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                                     subDirectory));
        directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                                     subDirectory));
    }

    private static string GetAppPath()
    {
        var appPath = AppDomain.CurrentDomain.BaseDirectory;
        if (appPath == null)
        {
            throw new InvalidOperationException("BaseDirectory is null");
        }

        return appPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                   ? appPath
                   : appPath + Path.DirectorySeparatorChar;
    }
}
