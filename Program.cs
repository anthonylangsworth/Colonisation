using System.Globalization;
using Newtonsoft.Json;
using CsvHelper;
using CsvHelper.Configuration;

// Download systemsWithCoordinates.json from https://www.edsm.net/en/nightly-dumps, specifically https://www.edsm.net/dump/systemsWithCoordinates.json.gz .
// Download systemsPopulated.json from https://www.edsm.net/en/nightly-dumps, specifically https://www.edsm.net/dump/systemsPopulated.json.gz .
// This files are large (12+ GB at time of writing for the systemsWithCoordinates.json) and changes as new systems are added. Therefore, downloading it is the best way to keep up-to-date.

// Sample (included for a format reference):
// [ {"id":8713,"id64":663329196387,"name":"4 Sextantis","coords":{"x":87.25,"y":96.84375,"z":-65},"date":"2015-05-12 15:29:33"} ]

using TextReader populatedSystemsReader = new StreamReader("systemsPopulated.json");
using JsonTextReader populatedSystemsJsonReader = new JsonTextReader(populatedSystemsReader);

List<StarSystemInfo> populatedSystems = [];
while (populatedSystemsJsonReader.Read())
{
    if (populatedSystemsJsonReader.TokenType == JsonToken.StartObject)
    {
        StarSystemInfo? starSystemInfo = new JsonSerializer().Deserialize<StarSystemInfo>(populatedSystemsJsonReader);
        if (starSystemInfo != null)
        {
            populatedSystems.Add(starSystemInfo);
        }
    }
}

PopulatedSpace populatedSpace = new(populatedSystems);
EDASpace edaSpace = new(populatedSystems);

using TextReader systemsReader = new StreamReader("systemsWithCoordinates.json");
using JsonTextReader jsonReader = new JsonTextReader(systemsReader);
List<StarSystemInfo> output = [];

while (jsonReader.Read())
{
    if (jsonReader.TokenType == JsonToken.StartObject)
    {
        StarSystemInfo? starSystemInfo = new JsonSerializer().Deserialize<StarSystemInfo>(jsonReader);
        if (starSystemInfo != null 
            && !populatedSpace.isPopulated(starSystemInfo) 
            && edaSpace.IsNear(starSystemInfo))
        {
            output.Add(starSystemInfo);
        }
    }
}

using CsvWriter csvWriter = new CsvWriter(Console.Out, CultureInfo.InvariantCulture, true);
csvWriter.Context.RegisterClassMap<StarSystemInfoClassMap>();
csvWriter.WriteRecords(output);

class EDASpace
{
    readonly private IList<StarSystemInfo> _edaStarSystems;
    private const string _minorFactionName = "EDA Kunti League";
    private const int _margin = 15;

    public EDASpace(ICollection<StarSystemInfo> populatedSystems)
    {
        _edaStarSystems = populatedSystems.Where(ssi => ssi.controllingFaction.name == _minorFactionName).ToList();
    }

    public bool IsNear(StarSystemInfo system)
    {
        return _edaStarSystems.Any(ssi => system.coords.x >= ssi.coords.x - _margin
            && system.coords.x <= ssi.coords.x + _margin
            && system.coords.y >= ssi.coords.y - _margin
            && system.coords.y <= ssi.coords.y + _margin
            && system.coords.z >= ssi.coords.z - _margin
            && system.coords.z <= ssi.coords.z + _margin);
    }

    // Is the system within a cube of 2 * margin of Kunti?
    //bool IsNear(StarSystemInfo system)
    //{
    //    // {"id":18517,"id64":9468121064873,"name":"Kunti","coords":{"x":88.65625,"y":-59.625,"z":-4.0625},"date":"2017-02-24 09:42:54"}

    //    double kuntiX = 88.65625;
    //    double kuntiY = -59.625;
    //    double kuntiZ = -4.0625;
    //    double margin = 15;

    //    return system.coords.x >= kuntiX - margin
    //        && system.coords.x <= kuntiX + margin
    //        && system.coords.y >= kuntiY - margin
    //        && system.coords.y <= kuntiY + margin
    //        && system.coords.z >= kuntiZ - margin
    //        && system.coords.z <= kuntiZ + margin;
    //}
}

class PopulatedSpace
{
    HashSet<string> _populatedSystemNames = [];

    public PopulatedSpace(ICollection<StarSystemInfo> populatedSystems)
    {
        _populatedSystemNames = populatedSystems.Select(ssi => ssi.name).ToHashSet();
    }

    public bool isPopulated(StarSystemInfo system)
    {
        return _populatedSystemNames.Contains(system.name);
    }
}

record Coords
{
    public double x = 0;
    public double y = 0;
    public double z = 0;
}

record MinorFaction
{
    public int id = 0;
    public string name = "";
}


record StarSystemInfo
{
    public int id = 0;
    public long? id64 = 0;
    public string name = "";
    public Coords coords = new();
    public DateTime date = DateTime.Now;
    public MinorFaction controllingFaction = new();
}

class StarSystemInfoClassMap : ClassMap<StarSystemInfo>
{
    public StarSystemInfoClassMap()
    {
        Map(ssi => ssi.id).Name("Id").Index(0);
        Map(ssi => ssi.name).Name("Name").Index(2);
        Map(ssi => ssi.coords.x).Name("X").Index(3);
        Map(ssi => ssi.coords.y).Name("Y").Index(4);
        Map(ssi => ssi.coords.z).Name("Z").Index(5);
    }
}