using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Person;

[Description("Search persons by personal identity number")]
public sealed class SearchCommand : TicCommand<SearchCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<QUERY>")]
        [Description("Personal identity number (e.g. 198207174171). Use % as wildcard")]
        public required string Query { get; set; }

        [CommandOption("--page-size")]
        [Description("Results per page")]
        [DefaultValue(100)]
        public int PageSize { get; set; } = 100;
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var result = await client.GetAsync($"datasets/persons?query={Uri.EscapeDataString(settings.Query)}&pageSize={settings.PageSize}");
        return ToObject(result);
    }
}
