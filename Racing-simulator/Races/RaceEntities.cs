using Spectre.Console;

namespace RacingSimulator.RaceEntities
{
    public class Race
    {
        public double Distance { get; }
        private readonly List<Transport.Transport> _participants = new List<Transport.Transport>();
        private Weather.IWeatherModifier _weatherModifier;

        public Race(double distance, Weather.IWeatherModifier weatherModifier)
        {
            Distance = distance;
            _weatherModifier = weatherModifier;
        }

        public void RegisterTransport(Transport.Transport transport)
        {
            _participants.Add(transport);
        }

        public static class RaceDistance
        {
            public static double GetRaceDistance()
            {
                while (true)
                {
                    AnsiConsole.MarkupLine("[bold cyan]Введите дистанцию гонки или 'q' для выхода:\n[/] ");
                    string distanceInput = Console.ReadLine().Trim().ToLower();

                    if (distanceInput == "q")
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine("[bold red]Выход из программы.[/]");
                        Environment.Exit(0);
                    }
                    else if (double.TryParse(distanceInput, out double distance) && distance > 0)
                    {
                        return distance;
                    }
                    else
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine("[bold red]Ошибка:[/] введите корректное значение дистанции.\n");
                    }
                }
            }
        }

        public Dictionary<string, double> GetResults()
        {
            var results = new Dictionary<string, double>();
            foreach (var transport in _participants)
            {
                results[transport.Name] = transport.CalculateTime(Distance, _weatherModifier);
            }
            return results;
        }
    }

    public class RaceResultsPrinter
    {
        public void PrintResults(Dictionary<string, double> results)
        {
            var winner = results.OrderBy(r => r.Value).First();
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold green]Победитель:[/] [bold yellow]{winner.Key}[/] [bold green]с временем[/] [bold yellow]{winner.Value:F2} ед.[/]\n");
        }
    }
}
