using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Company;

[Description("Get beneficial owners for a company")]
public sealed class BeneficialOwnersCommand : TicCommand<BeneficialOwnersCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<COMPANY_ID>")]
        [Description("Internal TIC company identifier")]
        public required int CompanyId { get; set; }
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var result = await client.GetAsync($"datasets/companies/{settings.CompanyId}/se/beneficial-owners");
        return ToObject(result);
    }
}
