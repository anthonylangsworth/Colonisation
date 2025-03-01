using Colonisation.Common;

class PopulatedSpace
{
    HashSet<string> _populatedSystemNames = [];

    public PopulatedSpace(ICollection<StarSystem> populatedSystems)
    {
        _populatedSystemNames = populatedSystems.Select(ssi => ssi.name).ToHashSet();
    }

    public bool Contains(StarSystem system)
    {
        return _populatedSystemNames.Contains(system.name);
    }
}