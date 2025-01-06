using CsvHelper.Configuration;

namespace Colonisation.Common;

public record ColonisationTarget
{
    public string name = "";
    public string nearestMinorFactionSystemName = "";
    public double distance = 0.0;
}

public class ColonisationTargetClassMap : ClassMap<ColonisationTarget>
{
    public ColonisationTargetClassMap()
    {
        Map(ct => ct.name).Name("Name").Index(0);
        Map(ct => ct.nearestMinorFactionSystemName).Name("Nearest EDA System").Index(1);
        Map(ct => ct.distance).Name("Distance").Index(2);
    }
}

public record PrioritisedColonisationTarget: ColonisationTarget
{
    public int points = 0;
    public string description = "";
}

public class PrioritisedColonisationTargetClassMap : ClassMap<PrioritisedColonisationTarget>
{
    public PrioritisedColonisationTargetClassMap()
    {
        Map(ct => ct.name).Name("Name").Index(0);
        Map(ct => ct.points).Name("Points").Index(1);
        Map(ct => ct.description).Name("Reason").Index(2);
        Map(ct => ct.nearestMinorFactionSystemName).Name("Nearest EDA System").Index(3);
        Map(ct => ct.distance).Name("Distance").Index(4);
    }
}