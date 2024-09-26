namespace RacingSimulator.Transport
{
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