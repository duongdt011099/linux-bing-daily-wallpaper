using System.Diagnostics;
using BingDailyWallpaper.Abstractions;

namespace BingDailyWallpaper.DesktopEnvironments;

public class CinnamonEnvironment : IDesktopEnvironment
{
    public void SetWallpaper(string imagePath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "gsettings",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        psi.ArgumentList.Add("set");
        psi.ArgumentList.Add("org.cinnamon.desktop.background");
        psi.ArgumentList.Add("picture-uri");
        psi.ArgumentList.Add($"file://{imagePath}");

        using var process = Process.Start(psi);
        process?.WaitForExit();
    }
}