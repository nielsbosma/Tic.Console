using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("ticio");

    config.AddBranch("company", company =>
    {
        company.SetDescription("Company search and lookup");
        company.AddCommand<Tic.Console.Commands.Company.GetCommand>("get");
        company.AddCommand<Tic.Console.Commands.Company.GetByIdCommand>("get-by-id");
        company.AddCommand<Tic.Console.Commands.Company.SearchCommand>("search");
        company.AddCommand<Tic.Console.Commands.Company.GraphCommand>("graph");
        company.AddCommand<Tic.Console.Commands.Company.TreeCommand>("tree");
        company.AddCommand<Tic.Console.Commands.Company.IntelligenceCommand>("intelligence");
        company.AddCommand<Tic.Console.Commands.Company.BeneficialOwnersCommand>("beneficial-owners");
        company.AddCommand<Tic.Console.Commands.Company.PartiesCommand>("parties");
        company.AddCommand<Tic.Console.Commands.Company.DebtorSummaryCommand>("debtor-summary");
        company.AddCommand<Tic.Console.Commands.Company.VehiclesCommand>("vehicles");
        company.AddCommand<Tic.Console.Commands.Company.PropertiesCommand>("properties");
    });

    config.AddBranch("person", person =>
    {
        person.SetDescription("Person search and lookup");
        person.AddCommand<Tic.Console.Commands.Person.SearchCommand>("search");
        person.AddCommand<Tic.Console.Commands.Person.GetCommand>("get");
        person.AddCommand<Tic.Console.Commands.Person.CompaniesCommand>("companies");
    });

    config.AddBranch("bankruptcy", bankruptcy =>
    {
        bankruptcy.SetDescription("Swedish bankruptcy records");
        bankruptcy.AddCommand<Tic.Console.Commands.Bankruptcy.SearchCommand>("search");
    });

    config.AddBranch("vehicle", vehicle =>
    {
        vehicle.SetDescription("Swedish vehicle registry");
        vehicle.AddCommand<Tic.Console.Commands.Vehicle.SearchCommand>("search");
    });
});

return app.Run(args);
