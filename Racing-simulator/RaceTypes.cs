using RacingSimulator.RaceEntities;
using Spectre.Console;

namespace RacingSimulator.RaceTypes
{
    public interface IRaceType
    {
        string Type { get; }
        void RegisterTransport(Race race, Transport.Transport transport);
    }

    public class GroundRace : IRaceType
    {
        public string Type => "ground";

        public void RegisterTransport(Race race, Transport.Transport transport)
        {
            if (transport is Transport.GroundTransport)
            {
                race.RegisterTransport(transport);
            }
            else
            {
                throw new InvalidOperationException($"{transport.Name} не может участвовать в наземной гонке.");
            }
        }
    }

    public class AirRace : IRaceType
    {
        public string Type => "air";

        public void RegisterTransport(Race race, Transport.Transport transport)
        {
            if (transport is Transport.AirTransport)
            {
                race.RegisterTransport(transport);
            }
            else
            {
                throw new InvalidOperationException($"{transport.Name} не может участвовать в воздушной гонке.");
            }
        }
    }

    public class MixedRace : IRaceType
    {
        public string Type => "mixed";

        public void RegisterTransport(Race race, Transport.Transport transport)
        {
            race.RegisterTransport(transport);
        }
    }

    public static class RaceTypeFactory
    {
        public static IRaceType CreateRaceType(int selection)
        {
            return selection switch
            {
                1 => new GroundRace(),
                2 => new AirRace(),
                3 => new MixedRace(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    public static class RaceTypeHelper
    {
        public static IRaceType GetRaceType()
        {
            while (true)
            {
                AnsiConsole.MarkupLine("[bold yellow]Добро пожаловать в симулятор гонок!\n[/]");
                AnsiConsole.MarkupLine("[bold cyan]Выберите тип гонки:[/]");
                Console.WriteLine("1) Наземный");
                Console.WriteLine("2) Воздушный");
                Console.WriteLine("3) Смешанный");
                Console.WriteLine("q) Выход\n");

                string raceTypeInput = Console.ReadLine().Trim().ToLower();

                if (raceTypeInput == "q")
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[bold red]Выход из программы.[/]");
                    Environment.Exit(0);
                }
                else if (int.TryParse(raceTypeInput, out int raceTypeSelection) && raceTypeSelection >= 1 && raceTypeSelection <= 3)
                {
                    return RaceTypeFactory.CreateRaceType(raceTypeSelection);
                }
                else
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[bold red]Ошибка:[/] введите корректное значение из диапазона 1 - 3 или (q)uit.\n");
                }
            }
        }
    }
}
