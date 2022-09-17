using IPGeoFencing.IP2Location.Abstractions;
using IPGeoFencing.IP2Location.DataModels;
using IPGeoFencing.IP2Location.Providers;

namespace IPGeoFencing.IP2Location.Factories
{
    public static class IP2LocationProviderFactory
    {
        public static async Task<IIP2LocationProvider> FromCSVFileAsync(string ip2LocationCSVFilePath)
        {
            ValidateFilePath(ip2LocationCSVFilePath);

            string csvText = await File.ReadAllTextAsync(ip2LocationCSVFilePath);                

            return GetIP2LocationInMemoryProvider(csvText);
        }

        public static IIP2LocationProvider FromCSVFile(string ip2LocationCSVFilePath)
        {
            ValidateFilePath(ip2LocationCSVFilePath);

            string csvText = File.ReadAllText(ip2LocationCSVFilePath);

            return GetIP2LocationInMemoryProvider(csvText);
        }

        private static void ValidateFilePath(string ip2LocationCSVFilePath)
        {
            if (ip2LocationCSVFilePath is null)
                throw new ArgumentNullException($"{nameof(IP2LocationProviderFactory)}->CreateInMemoryProviderFromCSVFile: {nameof(ip2LocationCSVFilePath)} parameter canot be null");

            if (!File.Exists(ip2LocationCSVFilePath))
                throw new FileNotFoundException($"{nameof(IP2LocationProviderFactory)}->CreateInMemoryProviderFromCSVFile: Unable to locate CSV file {ip2LocationCSVFilePath}");
        }

        private static IIP2LocationProvider GetIP2LocationInMemoryProvider(string csvText)
        {
            string[] lines = csvText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var ip2Locations = lines.Select(line => IPLocationModel.FromCSVLine(line.Trim()));

            return new IP2LocationInMemoryProvider(ip2Locations);
        }
    }
}
