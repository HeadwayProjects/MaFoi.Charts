using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MaFoi.Charts.Models
{
    public class AwsS3BucketProvider
    {

        private readonly string _awsApiKey;
        private readonly string _awsClientSecret;
        private readonly IAmazonS3 _amazonS3Client;
        public AwsS3BucketProvider()
        {
            _awsApiKey = "AKIAZT4KLL24N4VFV3H6";
            _awsClientSecret = "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";
            _amazonS3Client = new AmazonS3Client(_awsApiKey, _awsClientSecret, RegionEndpoint.APSouth1);
        }

        public async Task<string> UploadFile(Stream fileStream, UploadFilePathObject payload)
        {
            var fileUploadRequest = new PutObjectRequest()
            {
                BucketName = "mafoi",
                Key = $"{payload.BlobContainerName}/{payload.FileName}",
                InputStream = fileStream
            };

            try
            {
                await _amazonS3Client.PutObjectAsync(fileUploadRequest);
                var url = $"https://{fileUploadRequest.BucketName}.s3.{RegionEndpoint.APSouth1.SystemName}.amazonaws.com/{fileUploadRequest.Key}";
                return url;
            }
            catch (Exception e)
            {

            }
            return null;
        }       
    }
}