using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LogsArchiver.Filter
{
    public class Age : IFilter
    {
        private readonly IConfigurationRoot _configuration;

        public Age(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<LogFile>> Filter(IEnumerable<LogFile> files)
        {
            return files;
        }
    }
}
