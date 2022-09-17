using System.Net;

namespace IPGeoFencing.Networking
{
    public static class NetworkUtils
    {
        public static string? ResolveToIPAddress(string ipOrHostName)
        {
            IPAddress? ipAddress = null;

            if (!IPAddress.TryParse(ipOrHostName, out ipAddress))
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipOrHostName);

                ipAddress = hostEntry
                            .AddressList
                            .FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                && IsIPAddressLocal(a.ToString()));

                if (ipAddress == null)
                    ipAddress = hostEntry
                                .AddressList
                                .FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            }

            return ipAddress?.ToString();
        }

        public static bool IsIPAddressLocal(string ipAddress)
        {
            int[] ipParts = ipAddress.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => int.Parse(s)).ToArray();
            // in private ip range
            if (ipParts[0] == 10 ||
                (ipParts[0] == 192 && ipParts[1] == 168) ||
                (ipParts[0] == 172 && (ipParts[1] >= 16 && ipParts[1] <= 31)))
            {
                return true;
            }

            // IP Address is probably public.
            // This doesn't catch some VPN ranges like OpenVPN and Hamachi.
            return false;
        }

        public static long IPToNumber(string ipAddress)
        {
            // careful of sign extension: convert to uint first;
            // unsigned NetworkToHostOrder ought to be provided.
            return (long)(uint)IPAddress.NetworkToHostOrder((int)IPAddress.Parse(ipAddress).Address);
        }


        public static long IPToNumber(IPAddress ipAddress)
        {
            // careful of sign extension: convert to uint first;
            // unsigned NetworkToHostOrder ought to be provided.
            return (long)(uint)IPAddress.NetworkToHostOrder((int)ipAddress.Address);
        }

        public static string NumberToIP(long ipNumber)
        {
            return IPAddress.Parse(ipNumber.ToString()).ToString();
        }

        public static string? IP6ToIP4(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException($"{nameof(NetworkUtils)}->IP6ToIP4: a non empty ipAddress must be specified");

            string? result = null;

            IPAddress ip;
            if (IPAddress.TryParse(ipAddress, out ip))
            {
                // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                // This usually only happens when the browser is on the same machine as the server.
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    try
                    {
                        result = Dns.GetHostEntry(ipAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
                    }
                    catch (Exception) { } // Guard against DNS lookup failure
                }
            }

            return result ?? ipAddress;
        }
    }
}
