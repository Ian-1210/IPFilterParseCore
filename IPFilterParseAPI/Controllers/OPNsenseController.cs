//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IPFilterParseCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection;

namespace IPFilterParseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OPNsenseController : ControllerBase
    {
        [HttpGet]
        public string GetTest()
        {
            Global.logLevel = 3;
            Log log = Log.Instance;
            log.InitLog();
            log.LogLine("Test");
            return "Test";
        }

        [HttpGet]
        [Route("{textPath}")]
        public async Task<string> RunIpFilterParse([FromRoute] string textPath, int logLevel = 0)
        {
            Global.logLevel = logLevel;
            Log log = Log.Instance;
            log.InitLog();
            if (logLevel < 0)
            {
                log.LogLine("RunIpFilterParse API Test");
                return "RunIpFilterParse API Test";
            }
            log.LogLine("RunIpFilterParse controller before running MainProgram");
            log.LogLine("textPath=" + textPath);
            //log.LogLine("gateway=" + gateway);
            //log.LogLine("key=" + key);
            //log.LogLine("secret=" + secret);
            log.LogLine("logLevel=" + logLevel);
            await IPFilterParseCore.Program.MainProgram(textPath);     //gateway, key, secret
            log.LogLine("IpFilterParse complete.");
            return "IpFilterParse complete.";
        }
    }
}
