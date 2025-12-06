namespace AdventOfCode.Utils;

using System.Net;
using System.Reflection;

public class AocClient
{
    const string SESSION_COOKIE_FILE = "Session.user";

    static readonly HttpClient FHttpClient = CreateHttpClient();
    
    private static HttpClient CreateHttpClient()
    {
        var baseAddress = new Uri("https://adventofcode.com");
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(new Cookie("session", File.ReadAllText(SESSION_COOKIE_FILE), "/", ".adventofcode.com"));
        var client = new HttpClient(new HttpClientHandler() { CookieContainer = cookieContainer }) { BaseAddress = baseAddress };
        client.DefaultRequestHeaders.Add("User-Agent", GetUserAgent());
        return client;
    }

    private static string GetUserAgent()
    {
        var gitMail = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()?
            .FirstOrDefault(attr => attr.Key == "GitMail")?
            .Value;
        return $"AdventOfCodeCollection (github.com/boombuler/adventofcode by {gitMail})";
    }

    private static async Task GetAsync(string url, string cacheFile)
    {
        var response = await FHttpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            await using var inputData = await response.Content.ReadAsStreamAsync();
            string directory = Path.GetDirectoryName(cacheFile) ?? throw new InvalidOperationException();
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            await using var fs = File.Create(cacheFile);
            await inputData.CopyToAsync(fs);
        }
        else
            throw new InvalidOperationException($"Failed to load: {url}: {response.ReasonPhrase}");
    }
    
    public static async Task<string> GetPuzzleInput(int year, int day)
    {
        var puzzleStartTime = new DateTime(year, 12, day, 5, 0, 0, DateTimeKind.Utc);
        
        string relPath = Path.Combine("Input", year.ToString(), $"{day:d2}.txt");
        if (!File.Exists(relPath) && File.Exists(SESSION_COOKIE_FILE) && DateTime.UtcNow >= puzzleStartTime)
            await GetAsync($"/{year:d4}/day/{day}/input", relPath);
        
        if (!File.Exists(relPath))
            throw new InvalidOperationException("No Input available!");

        return (await File.ReadAllTextAsync(relPath)).ReplaceLineEndings("\n").TrimEnd('\n');
    }
}