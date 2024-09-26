using RacingSimulator.RaceEntities;
using static RacingSimulator.RaceEntities.Race;
using RacingSimulator.RaceTypes;
using RacingSimulator.Transport;
using RacingSimulator.Weather;
using Spectre.Console;

namespace RacingSimulatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var raceType = RaceTypeHelper.GetRaceType();
                AnsiConsole.Clear();

                double distance = RaceDistance.GetRaceDistance();
                AnsiConsole.Clear();

                var weatherCondition = WhatsTheWeatherLike.GetWeatherCondition();
                var weatherModifier = WeatherModifierFactory.CreateWeatherModifier(weatherCondition);

                var race = new Race(distance, weatherModifier);
                var availableTransports = AvailableTransports.GetAvailableTransports(raceType.Type);
                var registeredTransports = new HashSet<RacingSimulator.Transport.Transport>();

                Registration.RegisterTransports(availableTransports, registeredTransports, raceType, race);
                var results = race.GetResults();

                new RaceResultsPrinter().PrintResults(results);

                if (!PromptForNewRace()) break;
            }
        }

        static bool PromptForNewRace()
        {
            while (true)
            {
                AnsiConsole.MarkupLine("[bold cyan]Хотите начать новую гонку? (y/n)\n[/]");
                string response = Console.ReadLine().Trim().ToLower();

                if (response == "y")
                {
                    AnsiConsole.Clear();
                    return true;
                }
                else if (response == "n")
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[bold red]Выход из программы.[/]");
                    return false;
                }
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold red]Ошибка:[/] введите 'y' или 'n'.\n");
            }
        }
    }
}
