namespace RacingSimulator.Transport
{
    public interface ITimeCalculator
    {
        double CalculateTime(double distance, Weather.IWeatherModifier weatherModifier);
    }
}
