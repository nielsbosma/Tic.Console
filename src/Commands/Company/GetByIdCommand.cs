using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Company;

[Description("Look up company by internal TIC company ID")]
public sealed class GetByIdCommand : TicCommand<GetByIdCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<COMPANY_ID>")]
        [Description("Internal TIC company identifier")]
        public required int CompanyId { get; set; }
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var result = await client.SearchAsync(
            "companies", "*", "registrationNumber",
            filterBy: $"companyId:[{settings.CompanyId}]",
            perPage: 1);
        return ExtractSingleHit(result);
    }
}
