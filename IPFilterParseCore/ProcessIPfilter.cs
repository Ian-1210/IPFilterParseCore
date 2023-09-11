using System.Text;
using System.Text.RegularExpressions;

namespace IPFilterParseCore
{
    public class ProcessIPfilter
    {
        private List<IPfilterLine> ipFilterList = new List<IPfilterLine>();
        private string pathFile;
        private readonly string[] organisations = { "BayTSP", "Cyveillance", "Mediadefender", "Mediasentry", "Peer Media Technologies", "Mediaforce", "Internet Watch Foundation", "RIAA", "MPAA", "NameProtect" };
        private readonly Log log;

        public ProcessIPfilter()
        {
            log = Log.Instance;
        }


        internal void ReadIPfilter(string ipFilterPath)
        {
            log.LogLine("ProcessIPfilter.ReadIPfilter2 - Start");
            IPfilterLine ipFilterLine;
            char[] delimiterChars = { '-', ',' };

            using (StreamReader sr = File.OpenText(ipFilterPath))
            {
                string s = "";
                string[] ipFilterLineArray = { "" };

                while ((s = sr.ReadLine()) != null)
                {
                    ipFilterLineArray = s.Split(delimiterChars, 4);
                    if (ipFilterLineArray.Length == 4)
                    {
                        //Search ipFilterLineArray for each of the organisations we are looking for
                        foreach (string organisation in organisations)
                        {
                            if (ipFilterLineArray[3].ToLower().Contains(organisation.ToLower()))
                            {
                                ipFilterLine = new IPfilterLine();
                                ipFilterLine.IPAddressFrom = Regex.Replace(ipFilterLineArray[0].Trim(), "0*([0-9]+)", "${1}");
                                ipFilterLine.IPAddressTo = Regex.Replace(ipFilterLineArray[1].Trim(), "0*([0-9]+)", "${1}");
                                ipFilterLine.FilterLevel = ipFilterLineArray[2].Trim();
                                ipFilterLine.Organisation = ipFilterLineArray[3].Trim();
                                ipFilterList.Add(ipFilterLine);
                                log.LogLine("ipFilterLine: " + ipFilterLine.IPAddressFrom + "-" + ipFilterLine.IPAddressTo + ", " + ipFilterLine.FilterLevel + ", " + ipFilterLine.Organisation);
                                break;
                            }
                        }
                    }
                }
            }
            pathFile = Path.GetDirectoryName(ipFilterPath) + "\\ipfilter.txt";
            log.LogLine("pathFile=" + pathFile);
            log.LogLine("ProcessIPfilter.ReadIPfilter - End");
        }


        internal async Task ExtractSpies()      //string gatewayIPaddress, string key, string secret
        {
            log.LogLine("ProcessIPfilter.ExtractSpies - Start");
            //log.LogLine("gatewayIPaddress=" + gatewayIPaddress);
            //OPNsenseAPI opnSenseAPI = new OPNsenseAPI(gatewayIPaddress);     //key, secret
            OPNsenseAPI opnSenseAPI = new OPNsenseAPI();
            log.LogLine("pathFile=" + pathFile);
            StreamWriter sw;
            try
            {
                sw = new StreamWriter(pathFile);
            }
            catch (Exception ex)
            {
                log.LogLine("Exception on new StreamWriter(pathFile):");
                log.LogLine(ex.ToString());
                throw;
            }
            log.LogLine("StreamWriter created");
            Dictionary<string, List<IPAddressRange>> organisationIPs = new Dictionary<string, List<IPAddressRange>>();
            log.LogLine("organisationIPs.Count=" + organisationIPs.Count);
            StringBuilder ipAddressRanges = new StringBuilder();
            StringBuilder cidrRanges = new StringBuilder();

            //Populate organisation list with Org names
            log.LogLine("Populate organisation list with Org names");
            for (int i = 0; i < organisations.GetLength(0); i++)
            {
                organisationIPs.Add(organisations[i], new List<IPAddressRange>());
                log.LogLine("organisations[" + i + "]=" + organisations[i]);
            }

            //Populate organisation list with IPs to filter
            log.LogLine("Populate organisation list with IPs to filter");
            foreach (IPfilterLine ipFilterLine in ipFilterList)
            {
                log.LogLine("ipFilterLine.Organisation=" + ipFilterLine.Organisation);
                Dictionary<string, List<IPAddressRange>>.KeyCollection orgNames = organisationIPs.Keys;
                foreach (string orgName in orgNames.ToList())
                {
                    log.LogLine("orgName=" + orgName);
                    if (ipFilterLine.Organisation.ToLower().Contains(orgName.ToLower()))
                    {
                        log.LogLine("Organisation match");
                        //Add IP range
                        IPAddressRange ipAddressRange = new IPAddressRange();
                        log.LogLine("ipFilterLine.IPAddressFrom=" + ipFilterLine.IPAddressFrom);
                        log.LogLine("ipFilterLine.IPAddressTo=" + ipFilterLine.IPAddressTo);
                        ipAddressRange.SetIPAddressRange(ipFilterLine.IPAddressFrom,ipFilterLine.IPAddressTo);
                        log.LogLine("ipAddressRange=" + ipAddressRange.GetIPAddressRange());
                        organisationIPs[orgName].Add(ipAddressRange);
                        log.LogLine("orgName:" + orgName + ", Added ipAddressRange=" + ipAddressRange.GetIPAddressRange());
                        break;
                    }
                }
            }

            //Update Aliases from organisationIPs list
            log.LogLine("Update Aliases from organisationIPs list");
            foreach (KeyValuePair<string, List<IPAddressRange>> organisationIP in organisationIPs)
            {
                AliasUUID aliasUUID;
                AliasUtilList aliasUtilList;
                string aliasName = String.Concat(organisationIP.Key.Where(c => !Char.IsWhiteSpace(c))) + "_IPs";
                log.LogLine("aliasName=" + aliasName);

                aliasUUID = await opnSenseAPI.GetAliasUUID(aliasName);
                if (aliasUUID == null)
                    await opnSenseAPI.AddAlias(aliasName);
                else
                {
                    aliasUtilList = await opnSenseAPI.GetNetworksInAlias(aliasName);
                    for (int i = 0; i < aliasUtilList.total; i++)
                    {
                        log.LogLine("aliasUtilList.rows[i].ip=" + aliasUtilList.rows[i].ip);
                        await opnSenseAPI.DeleteNetworkFromAlias(aliasName, aliasUtilList.rows[i].ip);
                    }
                }

                foreach (IPAddressRange ipAddressRange in organisationIP.Value)
                {
                    string ipAddressRangeString = ipAddressRange.GetIPAddressRange();
                    ipAddressRanges.Append(ipAddressRangeString + ",");
                    log.LogLine("ipAddressRanges=" + ipAddressRanges.ToString());
                    List<string> cidrList = ConvertIPrangeToCIDR.iprange2cidr(ipAddressRange.FromIPaddress.ToString(), ipAddressRange.ToIPaddress.ToString());
                    foreach (string cidr in cidrList)
                    {
                        await opnSenseAPI.AddNetworkToAlias(aliasName, cidr);
                        cidrRanges.Append(cidr + ",");
                        log.LogLine("cidrRanges=" + cidrRanges.ToString());
                    }
                }
                if (ipAddressRanges.Length > 0)
                    ipAddressRanges.Length--;
                if (cidrRanges.Length > 0)
                    cidrRanges.Length--;
                sw.WriteLine(organisationIP.Key);
                sw.WriteLine(ipAddressRanges.ToString());
                sw.WriteLine(cidrRanges.ToString());
                if (organisationIP.Key != organisations[organisations.Length - 1])
                    sw.WriteLine();
                ipAddressRanges.Clear();
                cidrRanges.Clear();
            }
            sw.Close();
            await opnSenseAPI.Reconfigure();
            log.LogLine("ProcessIPfilter.ExtractSpies - End");
        }
    }
}
