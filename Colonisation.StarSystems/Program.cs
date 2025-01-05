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
logger.LogInformation("Parsed systemsPopulated.json");

PopulatedSpace populatedSpace = new(populatedSystems);
MinorFactionSpace minorFactionSpace = new(
    configuration["minorFactionName"] ?? "", 
    populatedSystems);
double colonisationRange = Convert.ToDouble(configuration["colonisationRange"]);
logger.LogInformation("Constructed minor faction space and populated space");

HashSet<StarSystemOutput> output =
    GetAllStarSystems("systemsWithCoordinates.json")
        .AsParallel()
        .Where(currentSystem => !populatedSpace.Contains(currentSystem))
        .Select(currentSystem =>
            {
                var (closestMinorFactionSystem, distance) = minorFactionSpace.Closest(currentSystem);
                return new StarSystemOutput
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
csvWriter.Context.RegisterClassMap<StarSystemOutputClassMap>();
csvWriter.WriteRecords(output.OrderBy(o => o.name));

IEnumerable<StarSystemInfo> GetAllStarSystems(string file)
{
    using TextReader systemsReader = new StreamReader(file);
    using JsonTextReader jsonReader = new(systemsReader);
    JsonSerializer jsonSerializer = new();

    while (jsonReader.Read())
    {
        if (jsonReader.TokenType == JsonToken.StartObject)
        {
            StarSystemInfo? currentSystem = jsonSerializer.Deserialize<StarSystemInfo>(jsonReader);
            if (currentSystem != null)
            {
                yield return currentSystem;
            }
        }
    }
}
