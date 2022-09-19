using System.Data;

namespace IPGeoFencing.IP2Location.DataModels
{
    public class IPLocationModel : LocationModel
    {
        public double IPFrom { get; private set; }
        public double IPTo { get; private set; }

        public static IPLocationModel Create(
                double ipFrom,
                double ipTo,
                string countryCode = "",
                string countryName = "",
                string regionName = "",
                string cityName = "",
                double latitude = 0,
                double longitude = 0,
                string zipCode = "",
                string timeZone = "")
        {

            if (ipFrom > ipTo)
                throw new InvalidDataException($"{nameof(ipFrom)} must be less then or equal to {nameof(ipTo)}");

            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException($"{nameof(latitude)} must be between -90 and 90");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException($"{nameof(longitude)} must be between -180 and 180");

            var ipLocation = new IPLocationModel
            {
                IPFrom = ipFrom,
                IPTo = ipTo,
                CountryCode = countryCode,
                CountryName = countryName,
                RegionName = regionName,
                CityName = cityName,
                Latitude = latitude,
                Longitude = longitude,
                ZipCode = zipCode,
                TimeZone = timeZone,
            };

            return ipLocation;
        }

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
                ipLocation = IPLocationModel.Create(
                    ipFrom: Convert.ToDouble(values[0]),
                    ipTo: Convert.ToDouble(values[1]),
                    countryCode: Convert.ToString(values[2]),
                    countryName: Convert.ToString(values[3]),
                    regionName: Convert.ToString(values[4]),
                    cityName: Convert.ToString(values[5]),
                    latitude: Convert.ToDouble(values[6]),
                    longitude: Convert.ToDouble(values[7]),
                    zipCode: Convert.ToString(values[8]),
                    timeZone: Convert.ToString(values[9])
                );
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"{nameof(IPLocationModel)}->FromCSVLine: Invalid Location CSV format on line:{Environment.NewLine}{csvLine}{Environment.NewLine}{ex}");
            }

            return ipLocation;
        }
    }
}
