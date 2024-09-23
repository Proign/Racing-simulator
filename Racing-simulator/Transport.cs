using RacingSimulator.RaceEntities;
using RacingSimulator.RaceTypes;
using Spectre.Console;

namespace RacingSimulator.Transport
{
    public interface ITimeCalculator
    {
        double CalculateTime(double distance, Weather.IWeatherModifier weatherModifier);
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

        public abstract double CalculateTime(double distance, Weather.IWeatherModifier weatherModifier);
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

        public override double CalculateTime(double distance, Weather.IWeatherModifier weatherModifier)
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

        public override double CalculateTime(double distance, Weather.IWeatherModifier weatherModifier)
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
            return distance > 100 ? 1 + 0.01 * (distance - 100) / 100 : 1.0;
        }
    }

    public class MagicBroom : AirTransport
    {
        public MagicBroom() : base("Метла", 20) { }

        protected override double GetAccelerationFactor(double distance)
        {
            return distance > 150 ? Math.Pow(distance / 150, 0.1) : 1.0;
        }
    }

    public class FlyingCarpet : AirTransport
    {
        public FlyingCarpet() : base("Ковер-самолет", 25) { }

        protected override double GetAccelerationFactor(double distance)
        {
            return distance > 120 ? 1 + Math.Log(distance / 120) : 1.0;
        }
    }

    public class FlyingShip : AirTransport
    {
        public FlyingShip() : base("Летучий корабль", 22) { }

        protected override double GetAccelerationFactor(double distance)
        {
            return distance > 200 ? 1 + 0.0001 * Math.Pow(distance - 200, 2) : 1.0;
        }
    }
}
