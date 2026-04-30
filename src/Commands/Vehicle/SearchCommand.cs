using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Vehicle;

[Description("Search Swedish vehicle registry")]
public sealed class SearchCommand : TicCommand<SearchCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<QUERY>")]
        [Description("Search query (licence plate, VIN, manufacturer, etc.)")]
        public required string Query { get; set; }

        [CommandOption("--query-by")]
        [Description("Fields to search (comma-separated). Default: licencePlate")]
        [DefaultValue("licencePlate")]
        public string QueryBy { get; set; } = "licencePlate";

        [CommandOption("--filter-by")]
        [Description("Filter expression")]
        public string? FilterBy { get; set; }

        [CommandOption("--sort-by")]
        [Description("Sort expression")]
        public string? SortBy { get; set; }

        [CommandOption("--per-page")]
        [Description("Results per page (default: 10)")]
        [DefaultValue(10)]
        public int PerPage { get; set; } = 10;
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var result = await client.SearchAsync(
            "vehicles/se",
            settings.Query,
            settings.QueryBy,
            filterBy: settings.FilterBy,
            sortBy: settings.SortBy,
            perPage: settings.PerPage);
        return ExtractHits(result);
    }
}
