using System.Globalization;
using Newtonsoft.Json;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Colonisation.Common;

// See README.md for details. Error handling is intentionally minimal to improve clarity and speed development.

// Sample of systemsWithCoordinates.json (included for a format reference and for Kunti's location):
// [ {"id":18517,"id64":9468121064873,"name":"Kunti","coords":{"x":88.65625,"y":-59.625,"z":-4.0625},"date":"2017-02-24 09:42:54"} ]

IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();

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
    configuration["minorFactionName"] ?? "", 
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
            && !populatedSpace.Contains(currentSystem))
        { 
            (StarSystemInfo system, double distance) = minorFactionSpace.Closest(currentSystem);
            if (distance <= Convert.ToDouble(configuration["distance"]))
            {

                output.Add(new StarSystemOutput
                {
                    name = currentSystem.name,
                    nearestMinorFactionSystemName = system.name,
                    distance = distance
                });
            }
        }
    }
}

using StreamWriter outputFile = new(configuration["outputFileName"] ?? "");
using CsvWriter csvWriter = new(outputFile, CultureInfo.InvariantCulture, true);
csvWriter.Context.RegisterClassMap<StarSystemOutputClassMap>();
csvWriter.WriteRecords(output.OrderBy(o => o.name));
