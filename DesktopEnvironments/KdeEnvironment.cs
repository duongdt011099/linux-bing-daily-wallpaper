using System.Diagnostics;
using BingDailyWallpaper.Abstractions;

namespace BingDailyWallpaper.DesktopEnvironments;

public class KdeEnvironment : IDesktopEnvironment
{
    public void SetWallpaper(string imagePath)
    {
        var script = $@"var Desktops = desktops();
        for (i=0;i<Desktops.length;i++) {{
            d = Desktops[i];
            d.wallpaperPlugin = ""org.kde.image"";
            d.currentConfigGroup = Array(""Wallpaper"",""org.kde.image"",""General"");
            d.writeConfig(""Image"",""file://{imagePath}"");
        }}";

        var psi = new ProcessStartInfo
        {
            FileName = "dbus-send",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        psi.ArgumentList.Add("--session");
        psi.ArgumentList.Add("--dest=org.kde.plasmashell");
        psi.ArgumentList.Add("--type=method_call");
        psi.ArgumentList.Add("/PlasmaShell");
        psi.ArgumentList.Add("org.kde.PlasmaShell.evaluateScript");
        psi.ArgumentList.Add($"string:{script}");

        using var process = Process.Start(psi);
        process?.WaitForExit();
    }
}