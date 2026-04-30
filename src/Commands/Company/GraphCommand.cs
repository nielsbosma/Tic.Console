using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Company;

[Description("Get ownership graph for a company")]
public sealed class GraphCommand : TicCommand<GraphCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<COMPANY_ID>")]
        [Description("Internal TIC company identifier")]
        public required int CompanyId { get; set; }

        [CommandOption("--include-beneficial-owners")]
        [Description("Include beneficial owners in graph")]
        [DefaultValue(true)]
        public bool IncludeBeneficialOwner { get; set; } = true;

        [CommandOption("--max-nodes")]
        [Description("Maximum total nodes in graph")]
        [DefaultValue(10000)]
        public int MaxTotalNodes { get; set; } = 10000;

        [CommandOption("--max-roles")]
        [Description("Exclude persons with more than N roles")]
        public int? LimitMaxRoleInCompanies { get; set; }
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var path = $"datasets/companies/{settings.CompanyId}/graph?includeBeneficialOwner={settings.IncludeBeneficialOwner}&maxTotalNodes={settings.MaxTotalNodes}";
        if (settings.LimitMaxRoleInCompanies.HasValue)
            path += $"&limitMaxRoleInCompanies={settings.LimitMaxRoleInCompanies.Value}";
        var result = await client.GetAsync(path);
        return ToObject(result);
    }
}
