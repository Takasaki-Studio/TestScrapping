using System.Net;
using System.Text;

var url = Environment.GetEnvironmentVariable("TS_URL");

if (string.IsNullOrWhiteSpace(url))
{
    Console.WriteLine("Variavel de ambiente TS_URL não encontrada");
    return;
}

var httpClientOptions = new HttpClientHandler
{
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
};

using var httpClient = new HttpClient(httpClientOptions);

const string outDir = "results";
if (!Directory.Exists(outDir))
{
    Directory.CreateDirectory(outDir);
}

var uri = new Uri(url);
var sitePath = Path.Join(outDir, uri.Host);

if (Directory.Exists(sitePath))
{
    Directory.Delete(sitePath, true);
}

Directory.CreateDirectory(sitePath);


for (var i = 0; i < 100; i++)
{
    var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:109.0) Gecko/20100101 Firefox/112.0");
    request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
    request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
    request.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.5,en;q=0.3");
    request.Headers.Add("Alt-Used", uri.Host);
    request.Headers.Add("Connection", "keep-alive");
    request.Headers.Add("Sec-Fetch-Dest", "document");
    request.Headers.Add("Sec-Fetch-Mode", "navigate");
    request.Headers.Add("Sec-Fetch-Site", "same-origin");
    request.Headers.Add("Upgrade-Insecure-Requests", "1");
    
    var response = await httpClient.SendAsync(request);
    var responsesBytes = await response.Content.ReadAsByteArrayAsync();
    var responseString = Encoding.UTF8.GetString(responsesBytes);
    var filePath = Path.Join(sitePath, $"{i}_C_{response.StatusCode}.html");
    await File.WriteAllTextAsync(filePath, responseString);
    await Task.Delay(500);
}