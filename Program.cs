using Newtonsoft.Json;

// Download systemsWithCoordinates.json from https://www.edsm.net/en/nightly-dumps, specifically https://www.edsm.net/dump/systemsWithCoordinates.json.gz .

// [ {"id":8713,"id64":663329196387,"name":"4 Sextantis","coords":{"x":87.25,"y":96.84375,"z":-65},"date":"2015-05-12 15:29:33"} ]

using TextReader systemsReader = new StreamReader("systemsWithCoordinates.json");
using JsonTextReader jsonReader = new JsonTextReader(systemsReader);
jsonReader.Read();
while (jsonReader.Read())
{
    JsonSerializer jsonSerialier = new();
    StarSystemInfo? starSystemInfo = jsonSerialier.Deserialize<StarSystemInfo>(jsonReader);
    if(starSystemInfo != null && NearKunti(starSystemInfo))
    {
        Console.Out.WriteLine(starSystemInfo);
    }
}

// Is the system within a cube of 2 * margin of Kunti?
bool NearKunti(StarSystemInfo system)
{
    // {"id":18517,"id64":9468121064873,"name":"Kunti","coords":{"x":88.65625,"y":-59.625,"z":-4.0625},"date":"2017-02-24 09:42:54"}

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

record Coords
{
    public double x = 0;
    public double y = 0;
    public double z = 0;
}

record StarSystemInfo
{
    public int id = 0;
    public long id64 = 0;
    public string name = "";
    public Coords coords = new();
    public DateTime date = DateTime.Now;
}
