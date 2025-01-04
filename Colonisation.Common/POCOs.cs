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