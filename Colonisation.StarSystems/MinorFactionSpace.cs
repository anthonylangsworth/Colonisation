using Colonisation.Common;

class MinorFactionSpace
{
    private readonly ISet<StarSystem> _starSystems;
    private static readonly string[] colonisationContactStationTypes = ["Outpost", "Coriolis Starport", "Ocellus Starport", "Asteroid base", "Orbis Starport"];

    public MinorFactionSpace(string minorFactionName, string minorFactionNativeStarSystemName, 
        ICollection<StarSystem> populatedSystems)
    {
        Name = minorFactionName.Trim();
        _starSystems = populatedSystems
                        .Where(ssi => ssi.stations.Any(s => IsControlledStation(s, Name)))
                        .ToHashSet();
        if (!_starSystems.Any())
        {
            throw new ArgumentException(
                $"The minor faction '{minorFactionName}' is not present in any star system", 
                nameof(minorFactionName));
        }

        try
        {
            string trimmedMinorFactionNativeStarSystemName = minorFactionNativeStarSystemName.Trim();
            NativeStarSystem = _starSystems.First(
                ss => string.Equals(ss.name, trimmedMinorFactionNativeStarSystemName, StringComparison.OrdinalIgnoreCase));
        }
        catch (InvalidOperationException)
        {
            throw new ArgumentException(
                $"The native star system '{minorFactionNativeStarSystemName}' is not a populated star system",
                nameof(minorFactionNativeStarSystemName));
        }
    }

    public string Name
    {
        get;
    }

    public StarSystem NativeStarSystem
    {
        get;
    }

    public (StarSystem, double) Closest(StarSystem system)
    {
        return _starSystems
                .Select(edass => (edass, Distance: Distance(system.coords, edass.coords)))
                .OrderBy(d => d.Distance)
                .FirstOrDefault();
    }

    public double DistanceFromNativeStarSystem(StarSystem system)
    {
        return Distance(system.coords, NativeStarSystem.coords);
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

