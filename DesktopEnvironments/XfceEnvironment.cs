using System.Diagnostics;
using BingDailyWallpaper.Abstractions;

namespace BingDailyWallpaper.DesktopEnvironments;

public class XfceEnvironment : IDesktopEnvironment
{
    public void SetWallpaper(string imagePath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "xfconf-query",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        psi.ArgumentList.Add("--channel");
        psi.ArgumentList.Add("xfce4-desktop");
        psi.ArgumentList.Add("--property");
        psi.ArgumentList.Add("/backdrop/screen0/monitor0/image-path");
        psi.ArgumentList.Add("--set");
        psi.ArgumentList.Add(imagePath);

        using var process = Process.Start(psi);
        process?.WaitForExit();
    }
}