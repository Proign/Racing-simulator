using RacingSimulator.RaceEntities;
using RacingSimulator.RaceTypes;
using Spectre.Console;

namespace RacingSimulator.Transport
{
    public abstract class Transport : ITimeCalculator
    {
        public string Name { get; }
        public double Speed { get; }

        protected Transport(string name, double speed)
        {
            Name = name;
            Speed = speed;
        }

        public abstract double CalculateTime(double distance, Weather.IWeatherModifier weatherModifier);
    }

    public class AvailableTransports
    {
        public static List<Transport> GetAvailableTransports(string raceType)
        {
            List<Transport> transports = new List<Transport>();

            if (raceType == "ground" || raceType == "mixed")
            {
                transports.Add(new SevenLeagueBoots());
                transports.Add(new PumpkinCarriage());
                transports.Add(new ChickenLeggedHut());
                transports.Add(new Centaur());
            }

            if (raceType == "air" || raceType == "mixed")
            {
                transports.Add(new BabaYagaMortar());
                transports.Add(new MagicBroom());
                transports.Add(new FlyingCarpet());
                transports.Add(new FlyingShip());
            }

            return transports;
        }

        public static void DisplayAvailableTransports(List<Transport> availableTransports, HashSet<Transport> registeredTransports)
        {
            AnsiConsole.MarkupLine("[bold cyan]Доступные средства передвижения:[/]");

            for (int i = 0; i < availableTransports.Count; i++)
            {
                var transport = availableTransports[i];
                AnsiConsole.MarkupLine($"[{(registeredTransports.Contains(transport) ? "grey" : "yellow")}]{i + 1}) {transport.Name}[/]");
            }

            if (availableTransports.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]Все транспортные средства зарегистрированы.[/]");
            }

            AnsiConsole.MarkupLine("[bold darkorange]s) Завершить регистрацию и начать гонку[/]");
            AnsiConsole.MarkupLine("[bold darkorange]q) Выход из программы[/]");

            if (registeredTransports.Count > 0)
            {
                AnsiConsole.MarkupLine("\n[bold green]Зарегистрированные средства передвижения:[/]");
                foreach (var transport in registeredTransports)
                {
                    AnsiConsole.MarkupLine($"- {transport.Name}");
                }
            }
        }

    }

    public class Registration
    {
        public static void RegisterTransports(List<RacingSimulator.Transport.Transport> availableTransports, HashSet<RacingSimulator.Transport.Transport> registeredTransports, IRaceType raceType, Race race)
        {
            while (true)
            {
                AvailableTransports.DisplayAvailableTransports(availableTransports, registeredTransports);

                AnsiConsole.MarkupLine("\n[bold cyan]Введите номер средства передвижения для регистрации или 's' для старта гонки, 'q' для выхода:\n[/]");
                string transportInput = Console.ReadLine().Trim().ToLower();

                if (transportInput == "q")
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[bold red]Выход из программы.[/]");
                    Environment.Exit(0);
                }

                if (transportInput == "s")
                {
                    if (registeredTransports.Count > 0)
                    {
                        break;
                    }
                    else
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine("[bold red]Ошибка:[/] необходимо зарегистрировать хотя бы одного участника.\n");
                        continue;
                    }
                }

                if (int.TryParse(transportInput, out int transportSelection) && transportSelection >= 1 && transportSelection <= availableTransports.Count)
                {
                    var selectedTransport = availableTransports[transportSelection - 1];

                    if (!registeredTransports.Contains(selectedTransport))
                    {
                        try
                        {
                            raceType.RegisterTransport(race, selectedTransport);
                            registeredTransports.Add(selectedTransport);
                            availableTransports.Remove(selectedTransport);
                            AnsiConsole.Clear();
                        }
                        catch (InvalidOperationException ex)
                        {
                            AnsiConsole.Clear();
                            AnsiConsole.MarkupLine($"[bold red]Ошибка:[/] {ex.Message}\n");
                        }
                    }
                    else
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine("[bold red]Ошибка:[/] данный транспорт уже зарегистрирован.\n");
                    }
                }
                else
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[bold red]Ошибка:[/] введите корректное действие из списка.\n");
                }
            }
        }
    }
}
