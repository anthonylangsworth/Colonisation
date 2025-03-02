using System.Globalization;
using Newtonsoft.Json;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Colonisation.Common;
using Microsoft.Extensions.Logging;
using static System.Collections.Specialized.BitVector32;

// See README.md for details. Error handling is intentionally minimal to improve clarity and speed development.

// Sample of systemsWithCoordinates.json for reference:
// [ {"id":18517,"id64":9468121064873,"name":"Kunti","coords":{"x":88.65625,"y":-59.625,"z":-4.0625},"date":"2017-02-24 09:42:54"} ]

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = loggerFactory.CreateLogger("Default");
JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings { Formatting = Formatting.Indented });
logger.LogInformation("Initialised");

HashSet<StarSystem> populatedSystems = [];
HashSet<Station> colonizingStations = [];
Task.WaitAll(
    [
        // Get a list of populated systems to exclude from colonisation targets
        Task.Run(() => populatedSystems = GetFromJson<StarSystem>(jsonSerializer, "systemsPopulated.json").ToHashSet()),
        // Exclude systems with a "System Colonisation Ship" from colonisation targets 
        Task.Run(() => colonizingStations = GetFromJson<Station>(jsonSerializer, "stations.json",
            station => station.systemName != null && station.name == "System Colonisation Ship").ToHashSet()),
]);
logger.LogInformation("Parsed input files");

MinorFactionSpace minorFactionSpace = new(
    configuration["minorFactionName"] ?? throw new ArgumentException("Missing minorFactionName in configuration"),
    populatedSystems);
StarSystemCollection populatedSpace = new(populatedSystems);
StarSystemCollection colonizingSpace = new(colonizingStations);
double colonisationRange = Convert.ToDouble(configuration["colonisationRange"]);
logger.LogInformation("Constructed minor faction space and populated space");

HashSet<ColonisationTarget> output =
    GetFromJson< StarSystem>(jsonSerializer, "systemsWithCoordinates.json")
        .AsParallel()
        .Where(currentSystem => !populatedSpace.Contains(currentSystem))
        .Where(currentSystem => !colonizingSpace.Contains(currentSystem))
        .Select(currentSystem =>
        {
            var (closestMinorFactionSystem, distance) = minorFactionSpace.Closest(currentSystem);
            return new ColonisationTarget
            {
                name = currentSystem.name,
                nearestMinorFactionSystemName = closestMinorFactionSystem.name,
                distance = distance
            };
        })
        .Where(sso => sso.distance <= colonisationRange)
        .ToHashSet();
logger.LogInformation("Found colonisable systems");

using StreamWriter outputFile = new(configuration["outputFileName"] ?? "");
using CsvWriter csvWriter = new(outputFile, CultureInfo.InvariantCulture, true);
csvWriter.Context.RegisterClassMap<ColonisationTargetClassMap>();
csvWriter.WriteRecords(output.OrderBy(o => o.name));

static IEnumerable<T> GetFromJson<T>(JsonSerializer jsonSerializer, string filename, 
        Predicate<T>? filter = default)
    where T: class
{
    using TextReader textReader = new StreamReader(filename);
    using JsonTextReader jsonReader = new(textReader);

    // This pattern, while more code than Deserialise<T[]>(), minimizes memory use on
    // multi GB input files.
    while (jsonReader.Read())
    {
        if (jsonReader.TokenType == JsonToken.StartObject)
        {
            T? current = jsonSerializer.Deserialize<T>(jsonReader);
            if (current != null && filter != null && filter(current))
            {
                yield return current;
            }
        }
    }
}
