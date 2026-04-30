using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Company;

[Description("Look up company by registration number")]
public sealed class GetCommand : TicCommand<GetCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<REG_NR>")]
        [Description("Organization/registration number (e.g. 556792-6687)")]
        public required string RegNr { get; set; }
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var regNr = settings.RegNr.Replace("-", "");
        var result = await client.SearchAsync("companies", regNr, "registrationNumber", perPage: 1);
        return ExtractSingleHit(result);
    }
}
