using System.Diagnostics;
using BingDailyWallpaper.Abstractions;

namespace BingDailyWallpaper.DesktopEnvironments;

public class GnomeEnvironment : IDesktopEnvironment
{
    public void SetWallpaper(string imagePath)
    {
        var uri = new Uri(imagePath);
        var command = $"gsettings set org.gnome.desktop.background picture-uri 'file://{uri.LocalPath}'";
        var psi = new ProcessStartInfo
        {
            FileName = "bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        process?.WaitForExit();
    }
}