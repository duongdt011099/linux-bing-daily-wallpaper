using BingDailyWallpaper.Abstractions;
using BingDailyWallpaper.Factories;
using BingWallpaper.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
      services.AddHttpClient("Bing", config =>
      {
        config.BaseAddress = new Uri("https://www.bing.com");
      }).RemoveAllLoggers();
      services.AddTransient<IBingWallpaperDownloader, BingWallpaperDownloader>();
    })
    .Build();

if (args.Length == 0)
{
  Console.WriteLine("Unknown parameter: No command provided, please use 'bwall --help' to see available commands");
  return;
}

var service = host.Services.GetRequiredService<IBingWallpaperDownloader>();

async Task HandleDownload(int idx = 0)
{
  var localPath = await service.DownloadWallpaperAsync(idx);
  if (localPath != null)
  {
    var desktop =
            Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP") ??
            Environment.GetEnvironmentVariable("XDG_SESSION_DESKTOP") ??
            Environment.GetEnvironmentVariable("DESKTOP_SESSION");

    if (string.IsNullOrEmpty(desktop))
    {
      Console.WriteLine("Unable to determine desktop environment, defaulting to GNOME.");
      desktop = "gnome";
    }

    var desktopEnvironment = DesktopEnvironmentFactory.Create(desktop);
    desktopEnvironment.SetWallpaper(localPath);
  }
  else
  {
    Console.WriteLine("Failed to download wallpaper.");
  }
}

switch (args[0])
{
  case "today":
    await HandleDownload(0);
    break;

  case "previous":
    if (args.Length > 1 && int.TryParse(args[1], out int previousDays))
    {
      await HandleDownload(previousDays);
    }
    else
    {
      await HandleDownload(1);
    }
    break;

  case "--help":
    Console.WriteLine("Available commands:");
    Console.WriteLine("  today     - Download today's wallpaper");
    Console.WriteLine("  previous <number> - Download the previous wallpaper, <number> is how many previous days to go back (default is 0)");
    Console.WriteLine("  --help    - Show this help message");
    break;

  default:
    Console.WriteLine("Unknown command, please use 'bwall --help' to see available commands");
    break;
}