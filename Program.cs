using System.Globalization;
using Newtonsoft.Json;
using CsvHelper;
using CsvHelper.Configuration;

// Download systemsWithCoordinates.json from https://www.edsm.net/en/nightly-dumps, specifically https://www.edsm.net/dump/systemsWithCoordinates.json.gz .
// Download systemsPopulated.json from https://www.edsm.net/en/nightly-dumps, specifically https://www.edsm.net/dump/systemsPopulated.json.gz .
// This files are large (12+ GB at time of writing for the systemsWithCoordinates.json) and changes as new systems are added. Therefore, downloading it is the best way to keep up-to-date.

// Sample of systemsWithCoordinates.json (included for a format reference and for Kunti's location):
// [ {"id":18517,"id64":9468121064873,"name":"Kunti","coords":{"x":88.65625,"y":-59.625,"z":-4.0625},"date":"2017-02-24 09:42:54"} ]

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
using JsonTextReader jsonReader = new(systemsReader);
List<Output> output = [];

while (jsonReader.Read())
{
    if (jsonReader.TokenType == JsonToken.StartObject)
    {
        StarSystemInfo? currentSystem = new JsonSerializer().Deserialize<StarSystemInfo>(jsonReader);
        if (currentSystem != null 
            && !populatedSpace.isPopulated(currentSystem) 
            && edaSpace.TryNear(currentSystem, out (StarSystemInfo system, double distance) nearestEdaSystem))
        {
            output.Add(new Output
            {
                name = currentSystem.name,
                nearestEdaSystemName = nearestEdaSystem.system.name,
                distance = nearestEdaSystem.distance
            });
        }
    }
}

using CsvWriter csvWriter = new CsvWriter(Console.Out, CultureInfo.InvariantCulture, true);
csvWriter.Context.RegisterClassMap<OutputClassMap>();
csvWriter.WriteRecords(output);

class EDASpace
{
    readonly private IList<StarSystemInfo> _edaStarSystems;

    // Possible generalization: Accept these as arguments in a constructor
    private const string _minorFactionName = "EDA Kunti League";
    private const int _margin = 10;

    public EDASpace(ICollection<StarSystemInfo> populatedSystems)
    {
        _edaStarSystems = populatedSystems.Where(ssi => ssi.factions.Any(f => f.name == _minorFactionName)).ToList();
    }

    public bool TryNear(StarSystemInfo system, out (StarSystemInfo, double) closestEdaSystem)
    {
        var nearbySystems = _edaStarSystems
                            .Select(edass => (edass, Distance: Distance(system.coords, edass.coords)))
                            .Where(d => d.Distance <= _margin)
                            .OrderBy(d => d.Distance);
        closestEdaSystem = nearbySystems.FirstOrDefault();
        return nearbySystems.Any();
    }

    public double Distance(Coords a, Coords b)
    {
        return Math.Sqrt(
            (a.x - b.x) * (a.x - b.x)
            + (a.y - b.y) * (a.y - b.y)
            + (a.z - b.z) * (a.z - b.z));
    }
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
    public List<MinorFaction> factions = new();
}

record Output
{
    public string name = "";
    public string nearestEdaSystemName = "";
    public double distance = 0.0;
}

class OutputClassMap : ClassMap<Output>
{
    public OutputClassMap()
    {
        Map(ssi => ssi.name).Name("Name").Index(0);
        Map(ssi => ssi.nearestEdaSystemName).Name("Nearest EDA System").Index(1);
        Map(ssi => ssi.distance).Name("Distance").Index(2);
    }
}
