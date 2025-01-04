using System.Globalization;
using Newtonsoft.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Colonisation.Common;

// See README.md for details. Error handling is intentionally minimal to improve clarity and speed development.

// Sample of systemsWithCoordinates.json (included for a format reference and for Kunti's location):
// [ {"id":18517,"id64":9468121064873,"name":"Kunti","coords":{"x":88.65625,"y":-59.625,"z":-4.0625},"date":"2017-02-24 09:42:54"} ]

IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();

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
MinorFactionSpace minorFactionSpace = new(
    configurationRoot["minorFactionName"] ?? "", 
    Convert.ToDouble(configurationRoot["distance"]), 
    populatedSystems);

using TextReader systemsReader = new StreamReader("systemsWithCoordinates.json");
using JsonTextReader jsonReader = new(systemsReader);
List<StarSystemOutput> output = [];

while (jsonReader.Read())
{
    if (jsonReader.TokenType == JsonToken.StartObject)
    {
        StarSystemInfo? currentSystem = new JsonSerializer().Deserialize<StarSystemInfo>(jsonReader);
        if (currentSystem != null
            && !populatedSpace.Contains(currentSystem)
            && minorFactionSpace.TryNear(currentSystem, out (StarSystemInfo system, double distance) nearestMinorFactionSystem))
        {
            output.Add(new StarSystemOutput
            {
                name = currentSystem.name,
                nearestEdaSystemName = nearestMinorFactionSystem.system.name,
                distance = nearestMinorFactionSystem.distance
            });
        }
    }
}

using StreamWriter outputFile = new(configurationRoot["outputFileName"] ?? "");
using CsvWriter csvWriter = new(outputFile, CultureInfo.InvariantCulture, true);
csvWriter.Context.RegisterClassMap<StarSystemOutputClassMap>();
csvWriter.WriteRecords(output.OrderBy(o => o.name));

class MinorFactionSpace
{
    readonly private IList<StarSystemInfo> _edaStarSystems;
    private readonly string _minorFactionName;
    private readonly double _distance;

    public MinorFactionSpace(string minorFactionName, double distance, ICollection<StarSystemInfo> populatedSystems)
    {
        if (distance <= 0)
        {
            throw new ArgumentException("Must be positive", nameof(distance));
        }

        _edaStarSystems = populatedSystems.Where(ssi => ssi.factions.Any(f => f.name == _minorFactionName)).ToList();
        _minorFactionName = minorFactionName;
        _distance = distance;
    }

    public bool TryNear(StarSystemInfo system, out (StarSystemInfo, double) closestEdaSystem)
    {
        var nearbySystems = _edaStarSystems
                            .Select(edass => (edass, Distance: Distance(system.coords, edass.coords)))
                            .Where(d => d.Distance <= _distance)
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

    // Thought: Use something like this to speed up TryNear
    public double GreatestExtent(Coords a, Coords b)
    {
        return new double[] {
            Math.Abs(a.x - b.x),
            Math.Abs(a.y - b.y),
            Math.Abs(a.z - b.z)}.Max();
    }
}

class PopulatedSpace
{
    HashSet<string> _populatedSystemNames = [];

    public PopulatedSpace(ICollection<StarSystemInfo> populatedSystems)
    {
        _populatedSystemNames = populatedSystems.Select(ssi => ssi.name).ToHashSet();
    }

    public bool Contains(StarSystemInfo system)
    {
        return _populatedSystemNames.Contains(system.name);
    }
}

class StarSystemOutputClassMap : ClassMap<StarSystemOutput>
{
    public StarSystemOutputClassMap()
    {
        Map(ssi => ssi.name).Name("Name").Index(0);
        Map(ssi => ssi.nearestEdaSystemName).Name("Nearest EDA System").Index(1);
        Map(ssi => ssi.distance).Name("Distance").Index(2);
    }
}


