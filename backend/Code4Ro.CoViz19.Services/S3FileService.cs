﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Code4Ro.CoViz19.Services.Options;
using Microsoft.Extensions.Options;

namespace Code4Ro.CoViz19.Services {
    public class S3FileService : IFileService
    {
        private readonly S3StorageOptions _s3Configuration;
        private readonly IAmazonS3 _client;
        public S3FileService(IOptions<S3StorageOptions> awsConfiguration)
        {
            _s3Configuration = awsConfiguration.Value;
            var awsOptions = new AWSOptions
            {
                Region = RegionEndpoint.GetBySystemName(_s3Configuration.Region),
                Credentials = new BasicAWSCredentials(_s3Configuration.ApiKey, _s3Configuration.Secret)
            };
            
            _client = awsOptions.CreateServiceClient<IAmazonS3>();
        }
        public string GetRawData()
        {
            var request = new GetObjectRequest {
                BucketName = _s3Configuration.BucketName,
                Key = _s3Configuration.LatestDataFileName
            };
            string responseBody;

            try
            {
                using (var response = _client.GetObjectAsync(request).Result)
                using (var responseStream = response.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    responseBody = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return responseBody;
        }

        public async Task SaveRawData(string fileContent)
        {
            var byteArray = Encoding.UTF8.GetBytes(fileContent);
            await using var stream = new MemoryStream(byteArray);

            var request = new TransferUtilityUploadRequest();
            using var utility = new TransferUtility(_client);
            request.BucketName = _s3Configuration.BucketName;
            request.Key = _s3Configuration.LatestDataFileName;
            request.InputStream = stream;

            await utility.UploadAsync(request);
        }
    }
}
