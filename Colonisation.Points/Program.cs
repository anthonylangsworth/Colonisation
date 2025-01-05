using Newtonsoft.Json;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Colonisation.Common;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.IO;

// See README.md for details. Error handling is intentionally minimal to improve clarity and speed development.

// Sample of systemsWithCoordinates.json for reference:
// [ {"id":18517,"id64":9468121064873,"name":"Kunti","coords":{"x":88.65625,"y":-59.625,"z":-4.0625},"date":"2017-02-24 09:42:54"} ]

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = loggerFactory.CreateLogger("Default");

using StreamReader inputFile = new(configuration["colonisationTargetsFileName"] ?? "");
using CsvReader csvReader = new(inputFile, CultureInfo.InvariantCulture);
csvReader.Context.RegisterClassMap<StarSystemOutputClassMap>();
List<StarSystemOutput> colonisationTargets = csvReader.GetRecords(new StarSystemOutput()).ToList();

JsonSerializer jsonSerializer = new();
using TextReader textReader = new StreamReader(configuration["bodiesDataFileName"] ?? "");
using JsonReader jsonReader = new JsonTextReader(textReader);
Dictionary<string, SystemBodiesInfo>? systemBodies =
    jsonSerializer.Deserialize<List<SystemBodiesInfo>>(jsonReader)
                 ?.ToDictionary(sbi => sbi.name, sbi => sbi);
if(systemBodies == null)
{
    throw new ArgumentException("Invalid system bodies file");
}
