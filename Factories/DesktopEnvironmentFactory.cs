using BingDailyWallpaper.Abstractions;
using BingDailyWallpaper.Constants;
using BingDailyWallpaper.DesktopEnvironments;

namespace BingDailyWallpaper.Factories;

public class DesktopEnvironmentFactory
{
    public static IDesktopEnvironment Create(string desktopEnvironment)
    {
        desktopEnvironment = desktopEnvironment.ToLower();

        if (desktopEnvironment == Environments.Kde)
        {
            return new KdeEnvironment();
        }
        else if (desktopEnvironment == Environments.Xfce)
        {
            return new XfceEnvironment();
        }
        else if (desktopEnvironment == Environments.Gnome)
        {
            return new GnomeEnvironment();
        }
        else if (desktopEnvironment == Environments.Mate)
        {
            return new MateEnvironment();
        }
        else if (desktopEnvironment == Environments.Cinnamon)
        {
            return new CinnamonEnvironment();
        }
        else
        {
            throw new NotSupportedException("Unsupported desktop environment.");
        }
    }
}