using GeoCoordinatePortable;
using System.Data;

namespace IPGeoFencing.IP2Location.DataModels
{
    public class IPLocationModel: LocationModel
    {
        public double IPFrom { get; private set; }
        public double IPTo { get; private set; }

        public static IPLocationModel FromCSVLine(string csvLine)
        {
            IPLocationModel ipLocation;

            var values = csvLine.Split(new string[] { "\",", ",\"" }, StringSplitOptions.None)
                .Select(s => s.Trim(new char[] { '\"', ' ' })
                .Replace("\\\"", "\"")).ToArray();

            if (values.Length != 10)
                throw new InvalidDataException($"{nameof(IPLocationModel)}->FromCSVLine: Invalid IP2Location CSV format: 10 columns expected but only {values.Length} columns found");
            
            try
            {
                ipLocation = new IPLocationModel
                {
                    IPFrom = Convert.ToDouble(values[0]),
                    IPTo = Convert.ToDouble(values[1]),
                    CountryCode = Convert.ToString(values[2]),
                    CountryName = Convert.ToString(values[3]),
                    RegionName = Convert.ToString(values[4]),
                    CityName = Convert.ToString(values[5]),
                    Latitude = Convert.ToDouble(values[6]),
                    Longitude = Convert.ToDouble(values[7]),
                    ZipCode = Convert.ToString(values[8]),
                    TimeZone = Convert.ToString(values[9])
                };
            } catch (Exception ex)
            {
                throw new InvalidDataException($"{nameof(IPLocationModel)}->FromCSVLine: Invalid Location CSV format on line:{Environment.NewLine}{csvLine}{Environment.NewLine}{ex}");
            }

            return ipLocation;
        }
    }
}
