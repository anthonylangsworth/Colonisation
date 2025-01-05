using Colonisation.Common;

class MinorFactionSpace
{
    private readonly ISet<StarSystemInfo> _starSystems;
    private readonly string _minorFactionName;

    public MinorFactionSpace(string minorFactionName, ICollection<StarSystemInfo> populatedSystems)
    {
        _minorFactionName = minorFactionName.Trim();
        _starSystems = populatedSystems.Where(ssi => ssi.factions.Any(f => string.Compare(f.name, _minorFactionName, true) == 0)).ToHashSet();
        if(!_starSystems.Any())
        {
            throw new ArgumentException("Not present in any star system", nameof(minorFactionName));
        }
    }

    public (StarSystemInfo, double) Closest(StarSystemInfo system)
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
}
