using System.Diagnostics;
using System.Net;
using System;

namespace IPFilterParseCore
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length >= 5)
                Global.logLevel = Convert.ToInt32(args[4]);
            else
                Global.logLevel = 0;
            Global.logLevel = 4;
            Log log = Log.Instance;
            log.InitLog();
            if (Global.logLevel < 0)
            {
                log.LogLine("RunIpFilterParse Console Test");
                return;
            }
            log.LogLine("Main start");
            log.LogLine("Global.debug=" + Global.debug);
            log.LogLine("args.Length=" + args.Length);

            if (args.Length < 1)  //4
            {
                log.LogLine("The Path argument is required.");
                //log.LogLine("The Path, Gateway, Key and Secret arguments are required.");
                return;
            }

            if (args.Length > 0) log.LogLine("args[0]=" + args[0]);
            if (args.Length > 1) log.LogLine("args[1]=" + args[1]);
            if (args.Length > 2) log.LogLine("args[2]=" + args[2]);
            if (args.Length > 3) log.LogLine("args[3]=" + args[3]);
            if (args.Length > 4) log.LogLine("args[4]=" + args[4]);

            string textPath = args[0];
            //string gateway = args[1];
            //string key = args[2];
            //string secret = args[3];

            await MainProgram(textPath);    //gateway, key, secret
            log.LogLine("Main end");
        }

        public static async Task MainProgram(string textPath)     //string gateway, string key, string secret
        {
            Log log = Log.Instance;
            log.LogLine("MainProgram start");

            DownloadBlocklist downloadBlocklist = new DownloadBlocklist();
            ProcessIPfilter processIPfilter = new ProcessIPfilter();
            string blocklistPath;
            if (String.IsNullOrEmpty(textPath))
                textPath = Global.programDirectory;
            log.LogLine("textPath=" + textPath);
            //log.LogLine("gateway=" + gateway);

            //Read apikey
            /*string[] lines = new string[2];
            string apikeyFilePath = Path.GetDirectoryName(textPath) + @"\apikey.txt";
            log.LogLine("apikeyFilePath=" + apikeyFilePath);
            using (StreamReader apikeyFile = new StreamReader(apikeyFilePath))
            {
                for (int l=0; l<2; l++)
                {
                    lines[l] = apikeyFile.ReadLine();
                    log.LogLine("line= " + lines[l]);
                }
            }*/

            if (!String.IsNullOrEmpty(textPath)) //&& !String.IsNullOrEmpty(gateway))
            {
                log.LogLine("After checking textPath and gateway are not null");
                FileAttributes attr;
                try
                {
                    attr = File.GetAttributes(textPath);
                }
                catch (Exception ex)
                {
                    log.LogLine("Exception on File.GetAttributes(textPath):");
                    log.LogLine(ex.ToString());
                    throw;
                }
                log.LogLine("After File.GetAttributes(textPath)");
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    log.LogLine("textPath is a directory");
                    blocklistPath = textPath + "\\BlocklistLevel1.zip";
                    log.LogLine("blocklistPath=" + blocklistPath);
                    await downloadBlocklist.DownloadIblocklistLevel1(blocklistPath);
                    blocklistPath = downloadBlocklist.ExtractBlocklist(blocklistPath);
                    log.LogLine("blocklistPath=" + blocklistPath);
                }
                else
                {
                    log.LogLine("textPath is a file");
                    blocklistPath = textPath;
                    log.LogLine("blocklistPath=" + blocklistPath);
                }
                log.LogLine("Before ReadIPfilter");
                processIPfilter.ReadIPfilter(blocklistPath);
                log.LogLine("After ReadIPfilter");
                //await processIPfilter.ExtractSpies(gateway);    //key,secret
                await processIPfilter.ExtractSpies();
                log.LogLine("After ExtractSpies");

                log.LogLine("MainProgram end");
            }
        }
    }
}
