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
    public List<MinorFaction> factions = [];
}

public record RingInfo
{
    public string name = "";
    public string type = "";
}

public record BodyInfo
{
    public int id = 0;
    public string name = "";
    public string type = "";
    public string subType = "";
    public double distanceToArrival = 0;
    public bool isLandable = false;
    public double gravity = 0;
    public string volcanismType = "";
    public string atmosphereType = "";
    public string terraformingState = "";
    public ICollection<RingInfo> rings = [];
}

public record SystemBodiesInfo
{
    public int id = 0;
    public string name = "";
    public ICollection<BodyInfo> bodies = [];
}