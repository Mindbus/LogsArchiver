using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LogsArchiver.Filter
{
    public class Zip : IFilter
    {
        private readonly IConfigurationRoot _configuration;
        private readonly int _index;

        public Zip(IConfigurationRoot configuration, int index)
        {
            _configuration = configuration;
            _index = index;
        }

        public async Task<IEnumerable<LogFile>> Filter(IEnumerable<LogFile> files)
        {
            var list = new List<LogFile>();
            foreach (var logFile in files)
            {
                var zipFileName = logFile.FullPath + ".zip";
                using (var zipFileStream = File.OpenWrite(zipFileName))
                using (var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(logFile.FullPath, logFile.FileName);
                }
                list.Add(new LogFile {FullPath = zipFileName, FileName = logFile.FileName, TimeStamp = logFile.TimeStamp});
                try
                {
#if !DEBUG
                    File.Delete(logFile.FullPath);
#endif
                }
                catch
                {
                    //soak it
                }
            }
            return list;
        }
    }
}
