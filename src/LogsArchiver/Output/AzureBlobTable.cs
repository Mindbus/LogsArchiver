using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LogsArchiver.Output
{
    public class AzureBlobTable :IOutput
    {
        private readonly IConfigurationRoot _configuration;

        public AzureBlobTable(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public Task Archive(LogFile logFile)
        {
            return Task.FromResult(0);
        }
    }
}
