using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LogsArchiver.Filter
{
    public class Zip : IFilter
    {
        private readonly IConfigurationRoot _configuration;

        public Zip(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<LogFile>> Filter(IEnumerable<LogFile> files)
        {
            return files;
        }
    }
}
