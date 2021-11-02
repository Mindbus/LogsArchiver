using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LogsArchiver
{
    public class AzureBlobHelper
    {
        public static CloudBlobClient GetClient(IConfigurationRoot configuration, string configprefix)
        {
            var connectionString = configuration[configprefix + ":connectionString"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount.CreateCloudBlobClient();
        }
    }
}
