using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Company;

[Description("Get vehicles owned by a company")]
public sealed class VehiclesCommand : TicCommand<VehiclesCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<COMPANY_ID>")]
        [Description("Internal TIC company identifier")]
        public required int CompanyId { get; set; }

        [CommandOption("--page")]
        [Description("Page number")]
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;

        [CommandOption("--page-size")]
        [Description("Results per page")]
        [DefaultValue(100)]
        public int PageSize { get; set; } = 100;
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var result = await client.GetAsync($"datasets/companies/{settings.CompanyId}/se/vehicles?pageNumber={settings.PageNumber}&pageSize={settings.PageSize}");
        return ToObject(result);
    }
}
