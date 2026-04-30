using System.ComponentModel;
using Spectre.Console.Cli;
using Tic.Console.Infrastructure;

namespace Tic.Console.Commands.Person;

[Description("Get person details by TIC person ID")]
public sealed class GetCommand : TicCommand<GetCommand.Settings>
{
    public sealed class Settings : GlobalSettings
    {
        [CommandArgument(0, "<PERSON_ID>")]
        [Description("Internal TIC person identifier")]
        public required int PersonId { get; set; }
    }

    protected override async Task<object> ExecuteAsync(TicApiClient client, Settings settings)
    {
        var result = await client.GetAsync($"datasets/persons/{settings.PersonId}");
        return ToObject(result);
    }
}
