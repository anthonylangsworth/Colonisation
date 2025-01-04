using Microsoft.Extensions.Configuration;
using CsvHelper;
using System.Globalization;
using Colonisation.Common;

IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();

using HttpClient httpClient = new HttpClient();

using StreamReader inputFile = new(configurationRoot["colonisationTargetsFileName"] ?? "");
using CsvReader csvReader = new(inputFile, CultureInfo.InvariantCulture);
csvReader.Context.RegisterClassMap<StarSystemOutputClassMap>();
foreach(StarSystemOutput starSystem in csvReader.EnumerateRecords(new StarSystemOutput()))
{
    string details = await httpClient.GetStringAsync($"https://www.edsm.net/api-system-v1/bodies?systemName={starSystem.name}");
    Console.Out.WriteLine(details);
}