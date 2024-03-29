﻿using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace LogsArchiver.Output
{
    public class AzureBlob : IOutput
    {
        private readonly IConfigurationRoot _configuration;
        private readonly Lazy<Task<BlobContainerClient>> _client;

        public AzureBlob(IConfigurationRoot configuration)
        {
            _configuration = configuration;
            _client = new Lazy<Task<BlobContainerClient>>(GetClient);
        }

        private async Task<BlobContainerClient> GetClient()
        {
            var containerName = _configuration["output:azureBlob:container"];
            var client = AzureBlobHelper.GetClient(_configuration, "output:azureBlob", containerName);
            var container = client.CreateIfNotExists().Value;
            return client;
        }

        public async Task Archive(LogFile logFile)
        {
            var container = await _client.Value;
            var blobName = GetRemotePath(logFile);
            using (var fileStream = System.IO.File.OpenRead(logFile.FullPath))
            {
                await _client.Value.Result.UploadBlobAsync(blobName, fileStream);
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
