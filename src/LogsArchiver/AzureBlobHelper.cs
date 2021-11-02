using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace LogsArchiver
{
    public class AzureBlobHelper
    {
        public static BlobContainerClient GetClient(IConfigurationRoot configuration, string configprefix, string containerName)
        {
            var connectionString = configuration[configprefix + ":connectionString"];
            var client = new BlobContainerClient(connectionString, containerName);
            return client;
        }
    }
}
