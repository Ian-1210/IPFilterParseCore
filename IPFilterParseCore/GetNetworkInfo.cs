using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace IPFilterParseCore
{
    internal class GetNetworkInfo
    {
        public static void GetNetworkInterfacesInfo()
        {
            NetworkInterface[] networkInterface = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in networkInterface)
            {
                IPInterfaceProperties ipInterfaceProperties = nic.GetIPProperties();

            }
        }

        public static List<IPAddress> GetGatewayIPaddresses()
        {
            List<IPAddress> gatewayIPaddresses = new List<IPAddress>();
            NetworkInterface[] networkInterface = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in networkInterface)
            {
                GatewayIPAddressInformation gatewayAddress = nic.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (gatewayAddress != null)
                    gatewayIPaddresses.Add(gatewayAddress.Address);
            }
            return gatewayIPaddresses;
        }
    }
}
