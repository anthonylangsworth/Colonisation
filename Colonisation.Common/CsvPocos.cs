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
        Map(ssi => ssi.name).Name("Name").Index(0);
        Map(ssi => ssi.nearestMinorFactionSystemName).Name("Nearest EDA System").Index(1);
        Map(ssi => ssi.distance).Name("Distance").Index(2);
    }
}