using Colonisation.Common;

// No sure whether we need this class. I was going to do more with it but the code went in a different direction.
class StarSystemCollection
{
    readonly HashSet<string> _systemNames = [];

    public StarSystemCollection(IEnumerable<StarSystem> systems)
    {
        _systemNames = systems.Select(ssi => ssi.name).ToHashSet();
    }

    public StarSystemCollection(IEnumerable<Station> stations)
    {
        _systemNames = stations.Select(ssi => ssi.systemName).ToHashSet();
    }

    public bool Contains(StarSystem system)
    {
        return _systemNames.Contains(system.name);
    }
}