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
    public bool? isLandadable = null;
    public double? gravity = 0;
    public string? atmosphereType = "";
    public string? terraformingState = "";
    public RingInfo[]? rings = null;
}

public record SystemBodiesInfo
{
    public int id = 0;
    public string name = "";
    public BodyInfo[] bodies = [];
}