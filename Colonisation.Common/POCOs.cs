using CsvHelper.Configuration;

namespace Colonisation.Common;

public record Coords
{
    public double x = 0;
    public double y = 0;
    public double z = 0;
}

public record MinorFaction
{
    public int id = 0;
    public string name = "";
}

public record StarSystemInfo
{
    public int id = 0;
    public long? id64 = 0;
    public string name = "";
    public Coords coords = new();
    public DateTime date = DateTime.UtcNow;
    public List<MinorFaction> factions = new();
}

public record StarSystemOutput
{
    public string name = "";
    public string nearestEdaSystemName = "";
    public double distance = 0.0;
}

public class StarSystemOutputClassMap : ClassMap<StarSystemOutput>
{
    public StarSystemOutputClassMap()
    {
        Map(ssi => ssi.name).Name("Name").Index(0);
        Map(ssi => ssi.nearestEdaSystemName).Name("Nearest EDA System").Index(1);
        Map(ssi => ssi.distance).Name("Distance").Index(2);
    }
}