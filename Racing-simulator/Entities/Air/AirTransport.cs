namespace RacingSimulator.Transport
{
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

}