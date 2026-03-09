using BingDailyWallpaper.Abstractions;
using BingWallpaper.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

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
  var response = await service.DownloadWallpaperAsync(idx);
  if (response != null)
  {
    var script = $@"var Desktops = desktops();
        for (i=0;i<Desktops.length;i++) {{
            d = Desktops[i];
            d.wallpaperPlugin = ""org.kde.image"";
            d.currentConfigGroup = Array(""Wallpaper"",""org.kde.image"",""General"");
            d.writeConfig(""Image"",""file://{response}"");
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