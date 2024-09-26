using RacingSimulator.RaceEntities;

namespace RacingSimulator.RaceTypes
{
    public interface IRaceType
    {
        string Type { get; }
        void RegisterTransport(Race race, Transport.Transport transport);
    }
}