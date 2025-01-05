using Colonisation.Common;

class PopulatedSpace
{
    HashSet<string> _populatedSystemNames = [];

    public PopulatedSpace(ICollection<StarSystemInfo> populatedSystems)
    {
        _populatedSystemNames = populatedSystems.Select(ssi => ssi.name).ToHashSet();
    }

    public bool Contains(StarSystemInfo system)
    {
        return _populatedSystemNames.Contains(system.name);
    }
}