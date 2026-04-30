using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tic.Console.Infrastructure;

public static class OutputHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly ISerializer YamlSerializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
        .Build();

    public static void Write(object data, string format)
    {
        var output = format.ToLowerInvariant() switch
        {
            "json" => JsonSerializer.Serialize(data, JsonOptions),
            "table" => FormatTable(data),
            _ => YamlSerializer.Serialize(data).TrimEnd()
        };

        System.Console.WriteLine(output);
    }

    public static void WriteError(string message, int code)
    {
        var error = new { error = message, code };
        var output = YamlSerializer.Serialize(error).TrimEnd();
        System.Console.Error.WriteLine(output);
    }

    private static string FormatTable(object data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array)
            return RenderArrayAsTable(items);

        if (root.ValueKind == JsonValueKind.Array)
            return RenderArrayAsTable(root);

        return RenderObjectAsKeyValue(root);
    }

    private static string RenderArrayAsTable(JsonElement array)
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);

        var rows = array.EnumerateArray().ToList();
        if (rows.Count == 0)
            return "(empty)";

        foreach (var prop in rows[0].EnumerateObject())
            table.AddColumn(new TableColumn(prop.Name).NoWrap());

        foreach (var row in rows)
        {
            var cells = row.EnumerateObject()
                .Select(p => Markup.Escape(FormatValue(p.Value)))
                .ToArray();
            table.AddRow(cells);
        }

        using var writer = new StringWriter();
        var console = AnsiConsole.Create(new AnsiConsoleSettings { Out = new AnsiConsoleOutput(writer) });
        console.Write(table);
        return writer.ToString().TrimEnd();
    }

    private static string RenderObjectAsKeyValue(JsonElement obj)
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.AddColumn("Key");
        table.AddColumn("Value");

        foreach (var prop in obj.EnumerateObject())
            table.AddRow(Markup.Escape(prop.Name), Markup.Escape(FormatValue(prop.Value)));

        using var writer = new StringWriter();
        var console = AnsiConsole.Create(new AnsiConsoleSettings { Out = new AnsiConsoleOutput(writer) });
        console.Write(table);
        return writer.ToString().TrimEnd();
    }

    private static string FormatValue(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.String => value.GetString() ?? "",
        JsonValueKind.Number => value.GetRawText(),
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        JsonValueKind.Null => "",
        _ => value.GetRawText()
    };
}
