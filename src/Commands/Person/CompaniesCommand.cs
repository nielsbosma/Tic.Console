using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Person;

[Description("Get all companies where a person has or had a role")]
public sealed class CompaniesCommand : TicCommand<CompaniesCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<PERSON_ID>")]
        [Description("Internal TIC person identifier")]
        public required int PersonId { get; set; }

        [CommandOption("--page-size")]
        [Description("Results per page")]
        [DefaultValue(100)]
        public int PageSize { get; set; } = 100;
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var result = await client.GetAsync($"datasets/persons/{settings.PersonId}/companies?pageSize={settings.PageSize}");
        return ToObject(result);
    }
}
