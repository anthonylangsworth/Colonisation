using System.Globalization;
using Newtonsoft.Json;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Colonisation.Common;
using Microsoft.Extensions.Logging;

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
    Task.Run(() => populatedSystems = [.. Json.Load<StarSystem>(jsonSerializer, "systemsPopulated.json")]),
    // Exclude systems with a "System Colonisation Ship" from colonisation targets 
    Task.Run(() => colonizingStations = [.. Json.Load<Station>(jsonSerializer, "stations.json",
        station => station.systemName != null && station.name == "System Colonisation Ship")]),
]);
logger.LogInformation("Parsed input files");

MinorFactionSpace minorFactionSpace = new(
    configuration["minorFactionName"] ?? throw new ArgumentException("Missing minorFactionName in configuration"),
    populatedSystems);
StarSystemCollection populatedSpace = new(populatedSystems);
StarSystemCollection colonizingSpace = new(colonizingStations);
double colonisationRange = Convert.ToDouble(configuration["colonisationRange"]);
logger.LogInformation("Constructed minor faction space, colonizing space and populated space");

HashSet<ColonisationTarget> output =
    Json.Load<StarSystem>(jsonSerializer, "systemsWithCoordinates.json")
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

using StreamWriter outputFile = new(configuration["outputFileName"]     
    ?? throw new ArgumentException("Missing outputFileName in configuration"));
using CsvWriter csvWriter = new(outputFile, CultureInfo.InvariantCulture, true);
csvWriter.Context.RegisterClassMap<ColonisationTargetClassMap>();
csvWriter.WriteRecords(output.OrderBy(o => o.name));
