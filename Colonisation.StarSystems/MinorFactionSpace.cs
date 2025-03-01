using Colonisation.Common;
using System.Linq;

class MinorFactionSpace
{
    private readonly ISet<StarSystem> _starSystems;
    private readonly string _minorFactionName;
    private static readonly string[] colonisationContactStationTypes = ["Outpost", "Coriolis Starport", "Ocellus Starport", "Asteroid base", "Orbis Starport"];

    public MinorFactionSpace(string minorFactionName, ICollection<StarSystem> populatedSystems)
    {
        _minorFactionName = minorFactionName.Trim();
        _starSystems = populatedSystems
                        .Where(ssi => ssi.stations.Any(s => IsControlledStation(s, _minorFactionName)))
                        .ToHashSet();
        if(!_starSystems.Any())
        {
            throw new ArgumentException("Not present in any star system", nameof(minorFactionName));
        }
    }

    public (StarSystem, double) Closest(StarSystem system)
    {
        return _starSystems
                .Select(edass => (edass, Distance: Distance(system.coords, edass.coords)))
                .OrderBy(d => d.Distance)
                .FirstOrDefault();
    }

    public static double Distance(Coords a, Coords b)
    {
        return Math.Sqrt(
            (a.x - b.x) * (a.x - b.x)
            + (a.y - b.y) * (a.y - b.y)
            + (a.z - b.z) * (a.z - b.z));
    }

    // Thought: Use something like this to speed up TryNear
    public static double GreatestExtent(Coords a, Coords b)
    {
        return new double[] {
            Math.Abs(a.x - b.x),
            Math.Abs(a.y - b.y),
            Math.Abs(a.z - b.z)}.Max();
    }

    public static bool IsControlledStation(Station station, string minorFactionName)
    {
        return string.Compare(station.controllingFaction.name, minorFactionName, true) == 0
            && colonisationContactStationTypes.Contains(station.type, StringComparer.OrdinalIgnoreCase);
    }
}

