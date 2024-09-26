namespace RacingSimulator.Transport
{
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
}