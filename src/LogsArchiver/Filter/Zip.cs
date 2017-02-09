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
        private readonly int _index;

        public Zip(IConfigurationRoot configuration, int index)
        {
            _configuration = configuration;
            _index = index;
        }

        public async Task<IEnumerable<LogFile>> Filter(IEnumerable<LogFile> files)
        {
            return files;
        }
    }
}
