using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Company;

[Description("Get properties owned by a company")]
public sealed class PropertiesCommand : TicCommand<PropertiesCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<COMPANY_ID>")]
        [Description("Internal TIC company identifier")]
        public required int CompanyId { get; set; }

        [CommandOption("--include-historical")]
        [Description("Include historical ownership (not just current)")]
        public bool IncludeHistorical { get; set; }
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var currentOnly = !settings.IncludeHistorical;
        var result = await client.GetAsync($"datasets/companies/{settings.CompanyId}/se/properties?includeOnlyCurrentOwnership={currentOnly}");
        return ToObject(result);
    }
}
