using IPGeoFencing.Engine.Builders;
using Microsoft.Extensions.Configuration;
using System.Net;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("IPGeoFencingDemo Started");

        try
        {
            var config = GetAppConfiguration();
            var dataFolder = GetDataFolderSetting(config);

            string ip2LocationCSVFile = Path.Combine(dataFolder, "IP2LOCATION-LITE-DB11.CSV");
            string geoJSONDemoFilePath = Path.Combine(dataFolder, "demo.geojson");

            var engine = new IPGeoFencingEngineBuilder()
                .AddIP2LocationFromCSVFile(ip2LocationCSVFile)
                .AddGeographicAreasFromGeoJSONFile(geoJSONDemoFilePath)
                .AddRule("New York",
                        predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "New York"); },
                        action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in New York State!"); })
                .AddRule("Montana",
                        predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "Montana"); },
                        action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in Montana!"); })
                .AddRule("Billings",
                        predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "Billings"); },
                        action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in Billings, MT"); })
                .AddRule("Montana but not Billings",
                        predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "Montana") && !areas.Any(A => A.Name == "Billings"); },
                        action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in Montana but not in Billings!"); })
                .AddRule("New York or Montana",
                        predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "Montana") || areas.Any(A => A.Name == "New York"); },
                        action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in New York State or Montana!"); })
                .AddDefaultAction((ip) => Console.WriteLine($"The IP Address: {ip} is outside all the areas provided"))
                .Build();


            IPAddress ipAddress;

            //Billings, MT IP Address
            ipAddress = IPAddress.Parse("98.127.147.57");
            engine.Run(ipAddress);

            //New York, NY IP Address
            ipAddress = IPAddress.Parse("172.254.112.210");
            engine.Run(ipAddress);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex}");
        }

        Console.WriteLine("IPGeoFencingDemo Finished");
        Console.WriteLine("Press <ENTER> to exit...");
        Console.ReadLine();
    }


    private static IConfigurationRoot? GetAppConfiguration()
    {
        var config = new ConfigurationBuilder()
                       .AddJsonFile("appsettings.json", false)
                       .Build();

        if (config is null)
            throw new ArgumentNullException("Unable to load configuration from file appsettings.json");

        return config;
    }

    private static string GetDataFolderSetting(IConfigurationRoot? config)
    {
        var dataFolder = config.GetValue<string>("DataFolder");

        if (string.IsNullOrWhiteSpace(dataFolder))
            throw new ArgumentNullException("Application setting DataFolder was not configured");

        if (!Directory.Exists(dataFolder))
            throw new DirectoryNotFoundException($"The data folder {dataFolder} configured in the application settings does not exists");

        return dataFolder;
    }
}