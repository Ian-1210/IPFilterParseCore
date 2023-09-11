using System.IO.Compression;

namespace IPFilterParseCore
{
    internal class DownloadBlocklist
    {
        Log log;

        public DownloadBlocklist()
        {
            log = Log.Instance;
        }

        public async Task DownloadIblocklistLevel1(string pZipPath)
        {
            string url = "http://list.iblocklist.com/?list=ydxerpxkpcfqjaybcssw&fileformat=dat&archiveformat=zip";
            HttpClient httpClient = new HttpClient();
            byte[] blocklistBytes = await httpClient.GetByteArrayAsync(url);

            FileStream fs = new FileStream(pZipPath, FileMode.Create);
            fs.Write(blocklistBytes, 0, blocklistBytes.Length);
            fs.Close();

            log.LogLine("File downloaded and saved as " + pZipPath);
        }

        public string ExtractBlocklist(string pZipPath)
        {
            string blocklistPath = null;
            using (ZipArchive zipArchive = ZipFile.OpenRead(pZipPath))
            {
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    log.LogLine("Extracted " + entry.Name);
                    blocklistPath = Path.GetDirectoryName(pZipPath) + "\\" + entry.Name;
                    entry.ExtractToFile(blocklistPath);
                }
            }
            log.LogLine("File extracted " + blocklistPath);
            return blocklistPath;
        }
    }
}
