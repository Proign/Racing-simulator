using Spectre.Console;

namespace RacingSimulator.Weather
{
    public static class WeatherModifierFactory
    {
        public static IWeatherModifier CreateWeatherModifier(WeatherCondition condition)
        {
            return condition switch
            {
                WeatherCondition.Sunny => new SunnyWeather(),
                WeatherCondition.Rainy => new RainyWeather(),
                WeatherCondition.Windy => new WindyWeather(),
                WeatherCondition.Foggy => new FoggyWeather(),
                _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, "Неизвестное погодное условие")
            };
        }
    }

    public class WhatsTheWeatherLike
    {
        public static WeatherCondition GetWeatherCondition()
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
                    return (WeatherCondition)(weatherSelection - 1); // Преобразование в перечисление
                }
                else
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[bold red]Ошибка:[/] введите корректное значение из диапазона 1 - 4.\n");
                }
            }
        }
    }
}
