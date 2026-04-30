using System.Text.Json;
using Spectre.Console.Cli;

namespace Tic.Console.Infrastructure;

public abstract class TicCommand<TSettings> : AsyncCommand<TSettings> where TSettings : GlobalSettings
{
    protected sealed override async Task<int> ExecuteAsync(CommandContext context, TSettings settings, CancellationToken cancellation)
    {
        try
        {
            var key = settings.ResolveApiKey();
            using var client = new TicApiClient(key, settings.Verbose);
            var result = await ExecuteAsync(client, settings);
            OutputHelper.Write(result, settings.Format);
            return 0;
        }
        catch (TicApiException ex)
        {
            OutputHelper.WriteError(ex.Message, ex.StatusCode);
            return ex.StatusCode;
        }
        catch (TicException ex)
        {
            OutputHelper.WriteError(ex.Message, 1);
            return 1;
        }
        catch (HttpRequestException ex)
        {
            OutputHelper.WriteError($"Connection failed: {ex.Message}", 2);
            return 2;
        }
    }

    protected abstract Task<object> ExecuteAsync(TicApiClient client, TSettings settings);

    protected static object ExtractSingleHit(JsonElement searchResult)
    {
        if (searchResult.TryGetProperty("hits", out var hits)
            && hits.ValueKind == JsonValueKind.Array
            && hits.GetArrayLength() > 0)
        {
            var first = hits[0];
            if (first.TryGetProperty("document", out var doc))
                return ConvertElement(doc);
        }

        throw new TicException("No results found");
    }

    protected static object ExtractHits(JsonElement searchResult)
    {
        var items = new List<object>();

        if (searchResult.TryGetProperty("hits", out var hits) && hits.ValueKind == JsonValueKind.Array)
        {
            foreach (var hit in hits.EnumerateArray())
            {
                if (hit.TryGetProperty("document", out var doc))
                    items.Add(ConvertElement(doc));
            }
        }

        var found = searchResult.TryGetProperty("found", out var f) ? f.GetInt64() : items.Count;
        return new Dictionary<string, object> { ["found"] = found, ["items"] = items };
    }

    protected static object ToObject(JsonElement element) => ConvertElement(element);

    private static object ConvertElement(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Object => element.EnumerateObject()
            .ToDictionary(p => p.Name, p => ConvertElement(p.Value)),
        JsonValueKind.Array => element.EnumerateArray()
            .Select(ConvertElement).ToList(),
        JsonValueKind.String => element.GetString()!,
        JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        _ => (object)null!
    };
}
