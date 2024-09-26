namespace RacingSimulator.Transport 
{
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
}