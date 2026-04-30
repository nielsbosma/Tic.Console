using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Company;

[Description("Search companies by name, address, phone, email, or other fields")]
public sealed class SearchCommand : TicCommand<SearchCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<QUERY>")]
        [Description("Search query (use * for wildcard)")]
        public required string Query { get; set; }

        [CommandOption("--query-by")]
        [Description("Fields to search (comma-separated). Default: names.nameOrIdentifier")]
        [DefaultValue("names.nameOrIdentifier")]
        public string QueryBy { get; set; } = "names.nameOrIdentifier";

        [CommandOption("--filter-by")]
        [Description("Filter expression (e.g. isCeased:false)")]
        public string? FilterBy { get; set; }

        [CommandOption("--sort-by")]
        [Description("Sort expression (e.g. registrationDate:desc)")]
        public string? SortBy { get; set; }

        [CommandOption("--per-page")]
        [Description("Results per page (default: 10)")]
        [DefaultValue(10)]
        public int PerPage { get; set; } = 10;
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var result = await client.SearchAsync(
            "companies",
            settings.Query,
            settings.QueryBy,
            filterBy: settings.FilterBy,
            sortBy: settings.SortBy,
            perPage: settings.PerPage);
        return ExtractHits(result);
    }
}
