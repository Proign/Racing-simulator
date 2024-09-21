using RacingSimulator;
using Spectre.Console;

namespace RacingSimulator
{
    public enum WeatherCondition
    {
        Sunny,
        Rainy,
        Windy,
        Foggy
    }

    public interface IWeatherModifier
    {
        double ModifyTime(double baseTime);
    }

    public class SunnyWeather : IWeatherModifier
    {
        public double ModifyTime(double baseTime) => baseTime;
    }

    public class RainyWeather : IWeatherModifier
    {
        public double ModifyTime(double baseTime) => baseTime * 1.10;
    }

    public class WindyWeather : IWeatherModifier
    {
        public double ModifyTime(double baseTime) => baseTime * 1.05;
    }

    public class FoggyWeather : IWeatherModifier
    {
        public double ModifyTime(double baseTime) => baseTime * 1.10;
    }

    public interface ITimeCalculator
    {
        double CalculateTime(double distance, IWeatherModifier weatherModifier);
    }

    public abstract class Transport : ITimeCalculator
    {
        public string Name { get; }
        public double Speed { get; }

        protected Transport(string name, double speed)
        {
            Name = name;
            Speed = speed;
        }

        public abstract double CalculateTime(double distance, IWeatherModifier weatherModifier);
    }

    public abstract class GroundTransport : Transport
    {
        public double RestInterval { get; }
        public double BaseRestDuration { get; }

        protected GroundTransport(string name, double speed, double restInterval, double baseRestDuration)
            : base(name, speed)
        {
            RestInterval = restInterval;
            BaseRestDuration = baseRestDuration;
        }

        private double CalculateRestDuration(int restCount)
        {
            return BaseRestDuration * Math.Log(restCount + 1) + BaseRestDuration;
        }

        public override double CalculateTime(double distance, IWeatherModifier weatherModifier)
        {
            double totalTime = 0;
            double traveledDistance = 0;
            int restCount = 0;

            while (traveledDistance < distance)
            {
                double travelBeforeRest = Speed * RestInterval;

                if (traveledDistance + travelBeforeRest >= distance)
                {
                    totalTime += (distance - traveledDistance) / Speed;
                    break;
                }

                totalTime += RestInterval;
                traveledDistance += travelBeforeRest;

                totalTime += CalculateRestDuration(restCount);

                restCount++;
            }

            double baseTime = totalTime;
            return weatherModifier.ModifyTime(baseTime);
        }
    }

    public abstract class AirTransport : Transport
    {
        protected AirTransport(string name, double speed) : base(name, speed) { }

        public override double CalculateTime(double distance, IWeatherModifier weatherModifier)
        {
            double acceleration = GetAccelerationFactor(distance);
            double finalSpeed = Speed;

            double timeToReachFinalSpeed = finalSpeed / acceleration;
            double distanceToReachFinalSpeed = (finalSpeed * finalSpeed) / (2 * acceleration);

            double baseTime;
            if (distance <= distanceToReachFinalSpeed)
            {
                baseTime = Math.Sqrt(2 * distance / acceleration);
            }
            else
            {
                double remainingDistance = distance - distanceToReachFinalSpeed;
                double timeToCoverRemainingDistance = remainingDistance / finalSpeed;
                baseTime = timeToReachFinalSpeed + timeToCoverRemainingDistance;
            }

            return weatherModifier.ModifyTime(baseTime);
        }

        protected abstract double GetAccelerationFactor(double distance);
    }

    public class SevenLeagueBoots : GroundTransport
    {
        public SevenLeagueBoots() : base("Сапоги-скороходы", 15, 5, 1) { }
    }

    public class PumpkinCarriage : GroundTransport
    {
        public PumpkinCarriage() : base("Карета-тыква", 10, 4, 1.5) { }
    }

    public class ChickenLeggedHut : GroundTransport
    {
        public ChickenLeggedHut() : base("Избушка на курьих ножках", 8, 6, 2) { }
    }

    public class Centaur : GroundTransport
    {
        public Centaur() : base("Кентавр", 18, 7, 1) { }
    }

    public class BabaYagaMortar : AirTransport
    {
        public BabaYagaMortar() : base("Ступа Бабы Яги", 12) { }

        protected override double GetAccelerationFactor(double distance)
        {
            if (distance > 100)
                return 1 + 0.01 * (distance - 100) / 100;
            return 1.0;
        }
    }

    public class MagicBroom : AirTransport
    {
        public MagicBroom() : base("Метла", 20) { }

        protected override double GetAccelerationFactor(double distance)
        {
            if (distance > 150)
                return Math.Pow(distance / 150, 0.1);
            return 1.0;
        }
    }

    public class FlyingCarpet : AirTransport
    {
        public FlyingCarpet() : base("Ковер-самолет", 25) { }

        protected override double GetAccelerationFactor(double distance)
        {
            if (distance > 120)
                return 1 + Math.Log(distance / 120);
            return 1.0;
        }
    }

    public class FlyingShip : AirTransport
    {
        public FlyingShip() : base("Летучий корабль", 22) { }

        protected override double GetAccelerationFactor(double distance)
        {
            if (distance > 200)
                return 1 + 0.0001 * Math.Pow(distance - 200, 2);
            return 1.0;
        }
    }

    public interface IRaceType
    {
        string Type { get; }
        void RegisterTransport(Race race, Transport transport);
    }

    public class GroundRace : IRaceType
    {
        public string Type => "ground";

        public void RegisterTransport(Race race, Transport transport)
        {
            if (transport is GroundTransport)
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

        public void RegisterTransport(Race race, Transport transport)
        {
            if (transport is AirTransport)
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

        public void RegisterTransport(Race race, Transport transport)
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

    public class Race
    {
        public double Distance { get; }
        private readonly List<Transport> _participants = new List<Transport>();
        private IWeatherModifier _weatherModifier;

        public Race(double distance, IWeatherModifier weatherModifier)
        {
            Distance = distance;
            _weatherModifier = weatherModifier;
        }

        public void RegisterTransport(Transport transport)
        {
            _participants.Add(transport);
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

namespace RacingSimulatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                IRaceType raceType = GetRaceType();
                AnsiConsole.Clear();

                double distance = GetRaceDistance();
                AnsiConsole.Clear();

                var weatherCondition = GetWeatherCondition();
                var weatherModifier = CreateWeatherModifier(weatherCondition);

                var race = new RacingSimulator.Race(distance, weatherModifier);
                var availableTransports = GetAvailableTransports(raceType.Type);
                var registeredTransports = new HashSet<RacingSimulator.Transport>();

                RegisterTransports(availableTransports, registeredTransports, raceType, race);

                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold darkslategray1]Гонка началась!\nЖдем победителя[/]");

                var results = race.GetResults();
                var resultsPrinter = new RacingSimulator.RaceResultsPrinter();
                AnsiConsole.MarkupLine("[bold blue]Гонка завершена![/]");
                resultsPrinter.PrintResults(results);

                if (!PromptForNewRace())
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[bold red]Выход из программы.[/]");
                    return;
                }
            }
        }

        private static RacingSimulator.IRaceType GetRaceType()
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
                    return RacingSimulator.RaceTypeFactory.CreateRaceType(raceTypeSelection);
                }
                else
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[bold red]Ошибка:[/] введите корректное значение из диапазона 1 - 3 или (q)uit.\n");
                }
            }
        }

        private static double GetRaceDistance()
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

        private static RacingSimulator.WeatherCondition GetWeatherCondition()
        {
            while (true)
            {
                AnsiConsole.MarkupLine("[bold cyan]Выберите погодные условия:[/] ");
                Console.WriteLine("1) Солнечно");
                Console.WriteLine("2) Дождь");
                Console.WriteLine("3) Ветер");
                Console.WriteLine("4) Туман");
                Console.WriteLine("q) Выход\n");

                string weatherInput = Console.ReadLine().Trim().ToLower();

                if (weatherInput == "q")
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[bold red]Выход из программы.[/]");
                    Environment.Exit(0);
                }
                else if (int.TryParse(weatherInput, out int weatherSelection) && weatherSelection >= 1 && weatherSelection <= 4)
                {
                    AnsiConsole.Clear();
                    return (RacingSimulator.WeatherCondition)weatherSelection - 1; // Преобразование в перечисление
                }
                else
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[bold red]Ошибка:[/] введите корректное значение из диапазона 1 - 4.\n");
                }
            }
        }

        private static RacingSimulator.IWeatherModifier CreateWeatherModifier(RacingSimulator.WeatherCondition condition)
        {
            return condition switch
            {
                RacingSimulator.WeatherCondition.Sunny => new RacingSimulator.SunnyWeather(),
                RacingSimulator.WeatherCondition.Rainy => new RacingSimulator.RainyWeather(),
                RacingSimulator.WeatherCondition.Windy => new RacingSimulator.WindyWeather(),
                RacingSimulator.WeatherCondition.Foggy => new RacingSimulator.FoggyWeather(),
                _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, "Неизвестное погодное условие")
            };
        }

        private static List<RacingSimulator.Transport> GetAvailableTransports(string raceType)
        {
            List<RacingSimulator.Transport> transports = new List<RacingSimulator.Transport>();

            if (raceType == "ground" || raceType == "mixed")
            {
                transports.Add(new RacingSimulator.SevenLeagueBoots());
                transports.Add(new RacingSimulator.PumpkinCarriage());
                transports.Add(new RacingSimulator.ChickenLeggedHut());
                transports.Add(new RacingSimulator.Centaur());
            }

            if (raceType == "air" || raceType == "mixed")
            {
                transports.Add(new RacingSimulator.BabaYagaMortar());
                transports.Add(new RacingSimulator.MagicBroom());
                transports.Add(new RacingSimulator.FlyingCarpet());
                transports.Add(new RacingSimulator.FlyingShip());
            }

            return transports;
        }

        private static void DisplayAvailableTransports(List<RacingSimulator.Transport> availableTransports, HashSet<RacingSimulator.Transport> registeredTransports)
        {
            AnsiConsole.MarkupLine("[bold cyan]Доступные средства передвижения:[/]");

            for (int i = 0; i < availableTransports.Count; i++)
            {
                AnsiConsole.MarkupLine($"[{(registeredTransports.Contains(availableTransports[i]) ? "grey" : "yellow")}]{i + 1}) {availableTransports[i].Name}[/]");
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

        private static void RegisterTransports(List<RacingSimulator.Transport> availableTransports, HashSet<RacingSimulator.Transport> registeredTransports, RacingSimulator.IRaceType raceType, RacingSimulator.Race race)
        {
            while (true)
            {
                DisplayAvailableTransports(availableTransports, registeredTransports);

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
                    RacingSimulator.Transport selectedTransport = availableTransports[transportSelection - 1];

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

        private static bool PromptForNewRace()
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
                    return false;
                }
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold red]Ошибка:[/] введите 'y' или 'n'.\n");
            }
        }
    }
}