using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LogsArchiver.Input
{
    public class FileSystem : IInput
    {
        private readonly IConfigurationRoot _configuration;

        public FileSystem(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }


        public async Task<IEnumerable<LogFile>> GetFiles()
        {
            var path = _configuration["input:fileSystem:path"];

            if (!Directory.Exists(path))
            {
                return new LogFile[] {};
            }

            Func<FileInfo, DateTime?> timestampDescriminator = null;
            var timestampDescriminatorName = _configuration["input:fileSystem:timestamp"];
            switch (timestampDescriminatorName)
            {
                case "FileName":
                    timestampDescriminator = GetDateFromFileNamePattern;
                    break;
                case "System":
                default:
                    timestampDescriminator = GetDateFromFileSystem;
                    break;
            }

            var folder = new DirectoryInfo(path);
            var files = folder.GetFiles();

            return files.Select(f => new LogFile
            {
                FileName = f.Name,
                FullPath = f.FullName,
                TimeStamp = timestampDescriminator(f)
            }).ToList(); //ToList to force eager execution of this non-async code
        }

        private DateTime? GetDateFromFileSystem(FileInfo file)
        {
            return file.LastWriteTime;
        }

        private DateTime? GetDateFromFileNamePattern(FileInfo file)
        {
            var pattern = _configuration["input:fileSystem:pattern"];
            var regEx = new Regex(pattern);
            var match = regEx.Match(file.Name);
            if (!match.Success)
            {
                return null;
            }
            var year = match.Groups["year"].Value;
            var month = match.Groups["month"].Value;
            var day = match.Groups["day"].Value;

            var composedDate = $"{year}{month}{day}";
            DateTime date;
            if (DateTime.TryParseExact(composedDate, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out date))
            {
                return date;
            }
            return null;
        }
    }
}
