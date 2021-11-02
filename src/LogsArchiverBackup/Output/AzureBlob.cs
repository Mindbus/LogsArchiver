using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LogsArchiver.Output
{
    public class AzureBlob : IOutput
    {
        private readonly IConfigurationRoot _configuration;
        private readonly Lazy<Task<CloudBlobContainer>> _container;

        public AzureBlob(IConfigurationRoot configuration)
        {
            _configuration = configuration;
            _container = new Lazy<Task<CloudBlobContainer>>(GetContainer);
        }

        private async Task<CloudBlobContainer> GetContainer()
        {
            var client = AzureBlobHelper.GetClient(_configuration, "output:azureBlob");
            var containerName = _configuration["output:azureBlob:container"];
            var container = client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }

        public async Task Archive(LogFile logFile)
        {
            var container = await _container.Value;
            var blobName = GetRemotePath(logFile);
            var blockBlob = container.GetBlockBlobReference(blobName);
            using (var fileStream = System.IO.File.OpenRead(logFile.FullPath))
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

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

        private string GetRemotePath(LogFile logFile)
        {
            var remotePath = _configuration["output:azureBlob:remotePath"];
            if (string.IsNullOrEmpty(remotePath))
            {
                return logFile.FileName;
            }
            remotePath = remotePath.TrimEnd('/');
            return $"{remotePath}/{logFile.FileName}";
        }
    }
}
