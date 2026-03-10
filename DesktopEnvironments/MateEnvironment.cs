using System.Diagnostics;
using BingDailyWallpaper.Abstractions;

namespace BingDailyWallpaper.DesktopEnvironments;

public class MateEnvironment : IDesktopEnvironment
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
        psi.ArgumentList.Add("org.mate.background");
        psi.ArgumentList.Add("picture-filename");
        psi.ArgumentList.Add(imagePath);

        using var process = Process.Start(psi);
        process?.WaitForExit();
    }
}