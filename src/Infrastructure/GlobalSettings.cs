using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Tic.Console.Infrastructure;

public class GlobalSettings : CommandSettings
{
    [CommandOption("--api-key")]
    [Description("TIC API key (overrides TIC_API_KEY env var)")]
    public string? ApiKey { get; set; }

    private static readonly HashSet<string> ValidFormats = new(StringComparer.OrdinalIgnoreCase) { "yaml", "json", "table" };

    [CommandOption("--format")]
    [Description("Output format: yaml, json, or table")]
    [DefaultValue("yaml")]
    public string Format { get; set; } = "yaml";

    public override ValidationResult Validate()
    {
        if (!ValidFormats.Contains(Format))
            return ValidationResult.Error($"Invalid format '{Format}'. Must be one of: yaml, json, table");
        return base.Validate();
    }

    [CommandOption("--no-color")]
    [Description("Disable colored output")]
    public bool NoColor { get; set; }

    [CommandOption("--verbose")]
    [Description("Print HTTP method, URL, and status code to stderr")]
    public bool Verbose { get; set; }

    public string ResolveApiKey()
    {
        var key = ApiKey;
        if (string.IsNullOrWhiteSpace(key))
            key = Environment.GetEnvironmentVariable("TIC_API_KEY");
        if (string.IsNullOrWhiteSpace(key))
            throw new TicException("API key not set. Provide --api-key or set the TIC_API_KEY environment variable.");
        return key;
    }
}
