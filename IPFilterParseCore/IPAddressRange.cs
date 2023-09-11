using System.Net;
using System.Text.RegularExpressions;

namespace IPFilterParseCore
{
    internal class IPAddressRange
    {
        public IPAddress FromIPaddress { get; set; }
        public IPAddress ToIPaddress { get; set; }

        public string GetIPAddressRange()
        {
            return FromIPaddress.ToString() + "-" + ToIPaddress.ToString();
        }

        public void SetIPAddressRange(IPAddress pFromIPaddress, IPAddress pToIPaddress)
        {
            FromIPaddress = pFromIPaddress;
            ToIPaddress = pToIPaddress;
        }

        public void SetIPAddressRange(string pFromIPaddress, string pToIPaddress)
        {
            string fromIPaddress = Regex.Replace(pFromIPaddress, "0*([0-9]+)", "${1}");
            string toIPaddress = Regex.Replace(pToIPaddress, "0*([0-9]+)", "${1}");
            SetIPAddressRange(IPAddress.Parse(fromIPaddress), IPAddress.Parse(toIPaddress));
        }
    }
}
