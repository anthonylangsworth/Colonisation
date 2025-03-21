﻿using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Colonisation.Common;
using Microsoft.Extensions.Logging;
using Colonisation.Points;
using CsvHelper;
using System.Globalization;

// See README.md for details. Error handling is intentionally minimal to improve clarity and speed development.

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = loggerFactory.CreateLogger("Default");

using StreamReader inputFile = new(configuration["colonisationTargetsFileName"] ?? "");
using CsvReader csvReader = new(inputFile, CultureInfo.InvariantCulture);
csvReader.Context.RegisterClassMap<ColonisationTargetClassMap>();
List<ColonisationTarget> colonisationTargets = csvReader.GetRecords<ColonisationTarget>().ToList();
logger.LogInformation("Loaded colonization targets");

JsonSerializer jsonSerializer = new();
using TextReader textReader = new StreamReader(configuration["bodiesDataFileName"] ?? "");
using JsonReader jsonReader = new JsonTextReader(textReader);
Dictionary<string, SystemBodies>? systemBodies =
    jsonSerializer.Deserialize<List<SystemBodies>>(jsonReader)
                 ?.ToDictionary(sbi => sbi.name, sbi => sbi);
if(systemBodies == null)
{
    throw new ArgumentException("Invalid system bodies file");
}
logger.LogInformation("Loaded system body information");

List<PrioritisedColonisationTarget> prioritisedColonisationTargets = [];
List<Rule> rules =
[
    new Bodies(),
    new LandableBodies(),
    // new MultipleStars(),
    new Rings(),
    new Belts(),
    // new TerraformableWorlds()
];
foreach(ColonisationTarget colonisationTarget in colonisationTargets)
{
    SystemBodies colonizationTargetBodies = systemBodies[colonisationTarget.name];

    IOrderedEnumerable<(double points, string description)> evaluatedRules = 
        rules
            .Select(r => r.Evaluate(colonisationTarget, colonizationTargetBodies))
            .Where(r => r.points > 0)
            .OrderByDescending(r => r.points);

    prioritisedColonisationTargets.Add(new PrioritisedColonisationTarget()
    {
        name = colonisationTarget.name,
        points = Math.Round(evaluatedRules.Sum(r => r.points), 1),
        description = string.Join("; ", evaluatedRules.Select(r => r.description)),
        nearestMinorFactionSystemName = colonisationTarget.nearestMinorFactionSystemName,
        distance = Math.Round(colonisationTarget.distance, 1),
        distanceFromNativeStarSystem = Math.Round(colonisationTarget.distanceFromNativeStarSystem, 1)
    });
}
logger.LogInformation("Run rules");

using StreamWriter outputFile = new(configuration["prioritisedColonisationTargetsFileName"] ?? "");
using CsvWriter csvWriter = new(outputFile, CultureInfo.InvariantCulture, true);
csvWriter.Context.RegisterClassMap<PrioritisedColonisationTargetClassMap>();
csvWriter.WriteRecords(prioritisedColonisationTargets.OrderBy(o => o.name).OrderByDescending(o => o.points));

