using Microsoft.Extensions.Configuration;
using CsvHelper;
using System.Globalization;
using Colonisation.Common;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net;

TimeSpan throttlePause = TimeSpan.FromSeconds(15);

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = loggerFactory.CreateLogger("Default");

using HttpClient httpClient = new();
JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings { Formatting = Formatting.Indented });

Dictionary<string, SystemBodies> output = Json.Load<SystemBodies>(jsonSerializer, configuration["bodiesDataFileName"] ?? "")
                                              .ToDictionary(sb => sb.name, sb => sb);

using StreamReader inputFile = new(configuration["colonisationTargetsFileName"] ?? "");
using CsvReader csvReader = new(inputFile, CultureInfo.InvariantCulture);
csvReader.Context.RegisterClassMap<ColonisationTargetClassMap>();
foreach(ColonisationTarget starSystem in csvReader.EnumerateRecords(new ColonisationTarget()))
{
    bool retry = true;
    if (output.ContainsKey(starSystem.name))
    {
        retry = false;
        logger.LogInformation("{starSystemName} is already loaded", starSystem.name);
    }

    while (retry)
    {
        retry = false;
        try
        {
            using Stream stream = await httpClient.GetStreamAsync($"https://www.edsm.net/api-system-v1/bodies?systemName={WebUtility.UrlEncode(starSystem.name)}");
            using TextReader textReader = new StreamReader(stream);
            using JsonReader jsonReader = new JsonTextReader(textReader);

            SystemBodies? systemBodiesInfo = jsonSerializer.Deserialize<SystemBodies>(jsonReader);
            if (systemBodiesInfo != null)
            {
                output.Add(systemBodiesInfo.name, systemBodiesInfo);
                logger.LogInformation("Loaded body data for {starSystemName}", starSystem.name);
            }
            else
            {
                logger.LogWarning("No body data for {starSystemName}", starSystem.name);
            }
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                logger.LogInformation("Pausing {pause} due to throttling", throttlePause);
                httpClient.CancelPendingRequests();
                Task.Delay(throttlePause).Wait();
                retry = true;
            }
            else
            {
                throw;
            }
        }
    }
}

using StreamWriter streamWriter = new(configuration["bodiesDataFileName"] ?? "");
using JsonWriter jsonWriter = new JsonTextWriter(streamWriter);
jsonSerializer.Serialize(jsonWriter, output.Values);
