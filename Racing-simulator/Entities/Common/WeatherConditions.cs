namespace RacingSimulator.Weather
{
    public enum WeatherCondition
    {
        Sunny,
        Rainy,
        Windy,
        Foggy
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
}