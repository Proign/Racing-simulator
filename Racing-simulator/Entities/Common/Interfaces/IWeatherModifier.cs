namespace RacingSimulator.Weather
{
    public interface IWeatherModifier
    {
        double ModifyTime(double baseTime);
    }
}