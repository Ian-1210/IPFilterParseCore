using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace IPFilterParseCore
{
    public class AliasUUID
    {
        public string UUID { get; set; }
    }

    internal class OPNsenseAPI
    {
        private readonly string ipAddress;
        private readonly string key;
        private readonly string secret;
        private readonly string baseURL;
        private Log log;

        public OPNsenseAPI()     //string ipAddress, string pKey, string pSecret
        {
            //key = pKey;
            //secret = pSecret;

            //Test VM
            ipAddress = "0.0.0.0";
            key = "key";
            secret = "secret";

            baseURL = "https://"+ ipAddress + "/api/firewall/";
            log = Log.Instance;
            log.LogLine("OPNsenseAPI baseURL=" + baseURL);
        }

        internal async Task<AliasUUID> GetAliasUUID(string pAliasName)
        {
            log.LogLine("OPNsenseAPI.GetAliasUUID - pAliasName=" + pAliasName);
            string url = baseURL + "alias/getAliasUUID/" + pAliasName;
            log.LogLine("url= " + url);

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(httpClientHandler);

            var authToken = Encoding.ASCII.GetBytes($"{key}:{secret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            //var result;
            HttpResponseMessage result;
            try
            {
                result = await client.GetAsync(url);
            }
            catch (Exception ex)
            {
                log.LogLine("Exception on client.GetAsync(url):");
                log.LogLine(ex.ToString());
                throw;
            }

            string content = await result.Content.ReadAsStringAsync();
            log.LogLine("content=" + content);

            AliasUUID aliasUUID = null;
            if (!content.Equals("[]"))
            {
                aliasUUID = JsonConvert.DeserializeObject<AliasUUID>(content);
                log.LogLine("aliasUUID.UUID=" + aliasUUID.UUID);
            }
            return aliasUUID;
        }
        
        internal async Task DelAlias(string pAliasName, AliasUUID pAliasUUID)
        {
            log.LogLine("OPNsenseAPI.DelAlias - pAliasName=" + pAliasName + ", pAliasUUID=" + pAliasUUID.UUID);
            string url = baseURL + "alias/delItem/" + pAliasUUID.UUID;
            log.LogLine("url= " + url);

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(httpClientHandler);

            var authToken = Encoding.ASCII.GetBytes($"{key}:{secret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            StringContent data = new StringContent("{ \"name\": \"" + pAliasName + "\" }");
            data.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await client.PostAsync(url, data);
            string content = await result.Content.ReadAsStringAsync();

            log.LogLine("content=" + content);
        }

        internal async Task AddAlias(string pAliasName)
        {
            log.LogLine("OPNsenseAPI.AddAlias - pAliasName=" + pAliasName);
            string url = baseURL + "alias/addItem";
            log.LogLine("url= " + url);

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(httpClientHandler);

            var authToken = Encoding.ASCII.GetBytes($"{key}:{secret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            StringContent data = new StringContent("{\"alias\":  { \"name\": \"" + pAliasName + "\", \"content\": \"\", \"enabled\" : \"1\", \"type\" : \"network\", \"description\": \"Auto added by IPfilterParse\"}}");
            data.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await client.PostAsync(url, data);
            string content = await result.Content.ReadAsStringAsync();

            log.LogLine("content=" + content);
        }

        internal async Task AddNetworkToAlias(string pAliasName, string pNetwork)
        {
            log.LogLine("OPNsenseAPI.AddNetworkToAlias - pAliasName=" + pAliasName + ", pNetwork=" + pNetwork);
            string url = baseURL + "alias_util/add/" + pAliasName;
            log.LogLine("url= " + url);

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(httpClientHandler);

            var authToken = Encoding.ASCII.GetBytes($"{key}:{secret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            StringContent data = new StringContent("{ \"address\": \"" + pNetwork + "\" }");
            data.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await client.PostAsync(url, data);
            string content = await result.Content.ReadAsStringAsync();

            log.LogLine("content=" + content);
        }

        internal async Task DeleteNetworkFromAlias(string pAliasName, string pNetwork)
        {
            log.LogLine("OPNsenseAPI.DeleteNetworkFromAlias - pAliasName=" + pAliasName + ", pNetwork=" + pNetwork);
            string url = baseURL + "alias_util/delete/" + pAliasName;
            log.LogLine("url= " + url);

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(httpClientHandler);

            var authToken = Encoding.ASCII.GetBytes($"{key}:{secret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            StringContent data = new StringContent("{ \"address\": \"" + pNetwork + "\" }");
            data.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await client.PostAsync(url, data);
            string content = await result.Content.ReadAsStringAsync();

            log.LogLine("content=" + content);
        }

        internal async Task<AliasUtilList> GetNetworksInAlias(string pAliasName)
        {
            log.LogLine("OPNsenseAPI.GetNetworksInAlias - pAliasName=" + pAliasName);
            string url = baseURL + "alias_util/list/" + pAliasName;
            log.LogLine("url= " + url);

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(httpClientHandler);

            var authToken = Encoding.ASCII.GetBytes($"{key}:{secret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            var result = await client.GetAsync(url);
            string content = await result.Content.ReadAsStringAsync();
            log.LogLine("content=" + content);

            AliasUtilList aliasUtilList = null;
            if (!content.Equals("[]"))
            {
                aliasUtilList = JsonConvert.DeserializeObject<AliasUtilList>(content);
                log.LogLine("aliasUtilList.total=" + aliasUtilList.total.ToString());
            }
            return aliasUtilList;
        }

        internal async Task<CategorySearchItem> GetCategories()
        {
            log.LogLine("OPNsenseAPI.GetCategories");
            string url = baseURL + "category/searchItem";
            log.LogLine("url= " + url);

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(httpClientHandler);

            var authToken = Encoding.ASCII.GetBytes($"{key}:{secret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            var result = await client.GetAsync(url);
            string content = await result.Content.ReadAsStringAsync();
            log.LogLine("content=" + content);

            CategorySearchItem categorySearchItem = null;
            if (!content.Equals("[]"))
            {
                categorySearchItem = JsonConvert.DeserializeObject<CategorySearchItem>(content);
                log.LogLine("categorySearchItem.Total=" + categorySearchItem.Total.ToString());
            }
            return categorySearchItem;
        }

        internal async Task Reconfigure()
        {
            log.LogLine("OPNsenseAPI.Reconfigure");
            string url = baseURL + "alias/reconfigure";
            log.LogLine("url= " + url);

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(httpClientHandler);

            var authToken = Encoding.ASCII.GetBytes($"{key}:{secret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            StringContent data = new StringContent("");
            var result = await client.PostAsync(url, data);
            string content = await result.Content.ReadAsStringAsync();

            log.LogLine("content=" + content);
        }
    }
}
