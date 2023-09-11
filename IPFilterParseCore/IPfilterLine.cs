namespace IPFilterParseCore
{
    internal class IPfilterLine
    {
        private string ipAddressFrom;
        public string IPAddressFrom { get { return ipAddressFrom; } set { ipAddressFrom = value; } }

        private string ipAddressTo;
        public string IPAddressTo { get { return ipAddressTo; } set { ipAddressTo = value; } }

        private string filterLevel;
        public string FilterLevel { get { return filterLevel; } set { filterLevel = value; } }

        private string organisation;
        public string Organisation { get { return organisation; } set { organisation = value; } }
    }
}
