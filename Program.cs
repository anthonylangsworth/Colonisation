using System.Text.Json;

// Download systemsWithCoordinates.json from https://www.edsm.net/en/nightly-dumps, specifically https://www.edsm.net/dump/systemsWithCoordinates.json.gz .

// [ {"id":8713,"id64":663329196387,"name":"4 Sextantis","coords":{"x":87.25,"y":96.84375,"z":-65},"date":"2015-05-12 15:29:33"} ]

// {"id":18517,"id64":9468121064873,"name":"Kunti","coords":{"x":88.65625,"y":-59.625,"z":-4.0625},"date":"2017-02-24 09:42:54"}

FileStream systemsFile = new("systemsWithCoordinates.json", FileMode.Open);
JsonDocument systemsJson = JsonDocument.Parse(systemsFile);
StarSystemInfo[] systems = systemsJson.Deserialize<StarSystemInfo[]>() ?? [];
foreach(StarSystemInfo system in systems.Where(NearKunti))
{
    Console.Out.WriteLine(system);
}

// Is the system within a cube of 2 * margin of Kunti?
bool NearKunti(StarSystemInfo system)
{
    double kuntiX = 88.65625;
    double kuntiY = -59.625;
    double kuntiZ = -4.0625;
    double margin = 15;

    return system.coords.x >= kuntiX - margin
        && system.coords.x <= kuntiX + margin
        && system.coords.y >= kuntiY - margin
        && system.coords.y <= kuntiY + margin
        && system.coords.z >= kuntiZ - margin
        && system.coords.z <= kuntiZ + margin;
}

class Coords
{
    public double x = 0;
    public double y = 0;
    public double z = 0;
}

class StarSystemInfo
{
    public int id = 0;
    public long id64 = 0;
    public string name = "";
    public Coords coords = new();
    public DateTime date = DateTime.Now;
}
