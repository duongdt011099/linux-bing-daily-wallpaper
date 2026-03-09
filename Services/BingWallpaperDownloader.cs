using System.Text.Json;
using BingDailyWallpaper.Abstractions;
using BingDailyWallpaper.Models;

namespace BingWallpaper.Services;

public class BingWallpaperDownloader : IBingWallpaperDownloader
{
  private const string BingWallpaperEndpoint = "/HPImageArchive.aspx?format=js&idx={0}&n=1&mkt=en-US";
  private readonly IHttpClientFactory _httpClientFactory;

  public BingWallpaperDownloader(IHttpClientFactory httpClientFactory)
  {
    _httpClientFactory = httpClientFactory;
  }

  public async Task<string?> DownloadWallpaperAsync(int idx = 0)
  {
    var httpClient = _httpClientFactory.CreateClient("Bing");
    var response = await httpClient.GetAsync(string.Format(BingWallpaperEndpoint, idx));
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    var bingWallpaperResponse = JsonSerializer.Deserialize<BingResponse>(content);

    if (bingWallpaperResponse == null || bingWallpaperResponse?.Images == null)
    {
      Console.WriteLine("Failed to parse Bing wallpaper response.");
      return null;
    }

    var image = bingWallpaperResponse.Images[0];
    var wallpaperUrl = $"https://www.bing.com{image.Url}";
    var wallpaperResponse = await httpClient.GetAsync(wallpaperUrl);
    wallpaperResponse.EnsureSuccessStatusCode();

    var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);;
    var directory = $"{home}/Pictures/Bing Wallpapers";

    if (!Directory.Exists(directory))
    {
      Directory.CreateDirectory(directory);
    }

    var filePath = $"{directory}/{image.FullStartDate}.jpg";

    if (!File.Exists(filePath))
    {
      await using var fs = new FileStream(filePath, FileMode.Create);
      await wallpaperResponse.Content.CopyToAsync(fs);
    }

    return filePath;
  }
}