using Microsoft.Extensions.Configuration;
using CsvHelper;
using System.Globalization;
using Colonisation.Common;
using Newtonsoft.Json;

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();

using HttpClient httpClient = new HttpClient();

using StreamWriter streanWriter = new StreamWriter(configuration["bodiesDataFileName"] ?? "");
using JsonWriter jsonWriter = new JsonTextWriter(streanWriter);
jsonWriter.WriteStartArray();

using StreamReader inputFile = new(configuration["colonisationTargetsFileName"] ?? "");
using CsvReader csvReader = new(inputFile, CultureInfo.InvariantCulture);
csvReader.Context.RegisterClassMap<StarSystemOutputClassMap>();
foreach(StarSystemOutput starSystem in csvReader.EnumerateRecords(new StarSystemOutput()))
{
    using Stream stream = await httpClient.GetStreamAsync($"https://www.edsm.net/api-system-v1/bodies?systemName={starSystem.name}");
    using TextReader textReader = new StreamReader(stream);
    using JsonReader jsonReader = new JsonTextReader(textReader);
    JsonSerializer jsonSerializer = new();

    SystemBodiesInfo? systemBodiesInfo = jsonSerializer.Deserialize<SystemBodiesInfo>(jsonReader);
    if(systemBodiesInfo != null)
    {
        jsonSerializer.Serialize(jsonWriter, systemBodiesInfo);
    }
}
jsonWriter.WriteEndArray();
