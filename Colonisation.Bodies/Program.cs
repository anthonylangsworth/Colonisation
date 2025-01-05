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
JsonSerializer jsonSerializer = new();

using StreamReader inputFile = new(configuration["colonisationTargetsFileName"] ?? "");
using CsvReader csvReader = new(inputFile, CultureInfo.InvariantCulture);
csvReader.Context.RegisterClassMap<StarSystemOutputClassMap>();
List<SystemBodiesInfo> output = [];
foreach(StarSystemOutput starSystem in csvReader.EnumerateRecords(new StarSystemOutput()))
{
    bool retry = true;
    while (retry)
    {
        retry = false;
        try
        {
            using Stream stream = await httpClient.GetStreamAsync($"https://www.edsm.net/api-system-v1/bodies?systemName={WebUtility.UrlEncode(starSystem.name)}");
            using TextReader textReader = new StreamReader(stream);
            using JsonReader jsonReader = new JsonTextReader(textReader);

            SystemBodiesInfo? systemBodiesInfo = jsonSerializer.Deserialize<SystemBodiesInfo>(jsonReader);
            if (systemBodiesInfo != null)
            {
                output.Add(systemBodiesInfo);
                logger.LogInformation("Loaded body data for {starSystemName}", starSystem.name);
            }
            else
            {
                logger.LogInformation("No body data for {starSystemName}", starSystem.name);
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

using StreamWriter streanWriter = new(configuration["bodiesDataFileName"] ?? "");
using JsonWriter jsonWriter = new JsonTextWriter(streanWriter);
jsonSerializer.Serialize(jsonWriter, output);
