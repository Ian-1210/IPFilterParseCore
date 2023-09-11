using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace IPFilterParseCore
{
    public sealed class Log
    {   //Singleton
        private static readonly Lazy<Log> lazy = new Lazy<Log>(() => new Log());
        public static Log Instance { get { return lazy.Value; } }
        private Log() { }

        readonly string logfilePath = Global.programDirectory + "\\IPFilterParseLog.txt";
        int consoleLog, debugLog, fileLog;

        public void LogLine(string textToLog)
        {
            DateTime dateTime = DateTime.Now;
            if (consoleLog == 1) Console.WriteLine(dateTime.ToString() + "  " + textToLog);
            if (debugLog == 2) Debug.WriteLine(dateTime.ToString() + "  " + textToLog);
            if (fileLog == 4) WriteLogLine(dateTime.ToString() + "  " + textToLog);
        }

        private void WriteLogLine(string textToLog)
        {
            using StreamWriter sw = new StreamWriter(logfilePath, true);
            sw.WriteLine(textToLog);
        }

        public void InitLog()
        {
            consoleLog = Global.logLevel & 1;
            debugLog = Global.logLevel & 2;
            fileLog = Global.logLevel & 4;
            if (fileLog == 4)
                try
                {
                    File.Delete(logfilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception on client.GetAsync(url):");
                    Console.WriteLine(ex.ToString());
                    throw;
                }
        }
    }
}
/* logLevel
 * -1 = Run test only
 * 0  = no logging
 * 1  = Console window
 * 2  = VS Debug window
 * 3  = 1 and 2
 * 4  = File IPFilterParseLog.txt
*/
