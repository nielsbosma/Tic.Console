using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tic.Console.Infrastructure;

public sealed class TicApiClient : IDisposable
{
    private const string BaseUrl = "https://api.tic.io/";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _http;
    private readonly bool _verbose;

    public TicApiClient(string apiKey, bool verbose = false)
    {
        _verbose = verbose;
        _http = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        _http.DefaultRequestHeaders.Add("x-api-key", apiKey);
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<JsonElement> SearchAsync(string collection, string query, string queryBy, string? filterBy = null, string? sortBy = null, int? perPage = null)
    {
        var sb = new StringBuilder($"search/{collection}?q={Uri.EscapeDataString(query)}&query_by={Uri.EscapeDataString(queryBy)}");
        if (!string.IsNullOrWhiteSpace(filterBy))
            sb.Append($"&filter_by={Uri.EscapeDataString(filterBy)}");
        if (!string.IsNullOrWhiteSpace(sortBy))
            sb.Append($"&sort_by={Uri.EscapeDataString(sortBy)}");
        if (perPage.HasValue)
            sb.Append($"&per_page={perPage.Value}");

        return await GetAsync(sb.ToString());
    }

    public async Task<JsonElement> GetAsync(string path)
    {
        var p = NormalizePath(path);
        LogRequest("GET", p);
        var response = await _http.GetAsync(p);
        return await HandleResponseAsync(response);
    }

    public async Task<JsonElement> PostAsync(string path, object? body = null)
    {
        var p = NormalizePath(path);
        LogRequest("POST", p);
        var content = body is not null
            ? new StringContent(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json")
            : null;
        var response = await _http.PostAsync(p, content);
        return await HandleResponseAsync(response);
    }

    private static string NormalizePath(string path) => path.TrimStart('/');

    private async Task<JsonElement> HandleResponseAsync(HttpResponseMessage response)
    {
        LogResponse(response);
        var body = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? 60;
            throw new TicApiException($"Rate limited (429). Retry after {retryAfter:F0}s", 2);
        }

        if (!response.IsSuccessStatusCode)
        {
            var message = TryExtractErrorMessage(body) ?? $"API error: {response.StatusCode}";
            throw new TicApiException(message, response.StatusCode >= HttpStatusCode.InternalServerError ? 2 : 1);
        }

        if (string.IsNullOrWhiteSpace(body))
            return JsonSerializer.SerializeToElement(new { success = true });

        return JsonSerializer.Deserialize<JsonElement>(body);
    }

    private static string? TryExtractErrorMessage(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.TryGetProperty("message", out var msg))
                return msg.GetString();
            if (root.TryGetProperty("error", out var err))
                return err.GetString();
            if (root.TryGetProperty("detail", out var detail))
                return detail.GetString();
            if (root.TryGetProperty("title", out var title))
            {
                var text = title.GetString() ?? "Validation error";
                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                {
                    var details = errors.EnumerateObject()
                        .SelectMany(p => p.Value.EnumerateArray().Select(v => $"{p.Name}: {v.GetString()}"));
                    text += " - " + string.Join("; ", details);
                }
                return text;
            }
        }
        catch { }
        return null;
    }

    private void LogRequest(string method, string path)
    {
        if (_verbose)
            System.Console.Error.WriteLine($">> {method} {BaseUrl}{path}");
    }

    private void LogResponse(HttpResponseMessage response)
    {
        if (_verbose)
            System.Console.Error.WriteLine($"<< {(int)response.StatusCode} {response.StatusCode}");
    }

    public void Dispose() => _http.Dispose();
}
