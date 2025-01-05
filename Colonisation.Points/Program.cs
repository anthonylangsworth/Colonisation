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