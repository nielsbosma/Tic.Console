using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Bankruptcy;

[Description("Search Swedish bankruptcy records")]
public sealed class SearchCommand : TicCommand<SearchCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<QUERY>")]
        [Description("Search query (registration number, company name, or * for wildcard)")]
        public required string Query { get; set; }

        [CommandOption("--query-by")]
        [Description("Fields to search (comma-separated). Default: registrationNumber")]
        [DefaultValue("registrationNumber")]
        public string QueryBy { get; set; } = "registrationNumber";

        [CommandOption("--filter-by")]
        [Description("Filter expression (e.g. bankruptcyStatusCode:[20])")]
        public string? FilterBy { get; set; }

        [CommandOption("--sort-by")]
        [Description("Sort expression (e.g. initiatedDate:desc)")]
        public string? SortBy { get; set; }

        [CommandOption("--per-page")]
        [Description("Results per page (default: 10)")]
        [DefaultValue(10)]
        public int PerPage { get; set; } = 10;
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var result = await client.SearchAsync(
            "companies/bankruptcies/se",
            settings.Query,
            settings.QueryBy,
            filterBy: settings.FilterBy,
            sortBy: settings.SortBy,
            perPage: settings.PerPage);
        return ExtractHits(result);
    }
}
