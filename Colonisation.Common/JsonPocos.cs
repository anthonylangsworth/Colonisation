namespace Colonisation.Common;

public record Coords
{
    public double x = 0;
    public double y = 0;
    public double z = 0;
}

public record MinorFaction
{
    public int? id = null; // Will be null for engineer bases
    public string name = "";
}

public record StarSystem
{
    public int id = 0;
    public long? id64 = 0;
    public string name = "";
    public Coords coords = new();
    public DateTime date = DateTime.UtcNow;
    public ICollection<MinorFaction> factions = [];
    public ICollection<Station> stations = [];
}

public record Ring
{
    public string name = "";
    public string type = "";
}

public record Body
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
    public ICollection<Ring> rings = [];
}

public record Belt
{
    public int id = 0;
    public string name = "";
    public string type = "";
}

public record Station
{
    public int id = 0;
    public string name = "";
    public string type = "";
    public string systemName = "";
    public MinorFaction controllingFaction = new();
}

public record SystemBodies
{
    public int id = 0;
    public string name = "";
    public ICollection<Body> bodies = [];
    public ICollection<Belt> belts= [];
}