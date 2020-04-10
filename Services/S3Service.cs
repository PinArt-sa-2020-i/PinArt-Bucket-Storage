using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PinArt_Bucket_Storage.Models;

namespace PinArt_Bucket_Storage.Services
{

    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _client;

        public S3Service(IAmazonS3 client)
        {
            _client = client;
        }

        public async Task<S3Response> CreateBucketAsync(string bucketName)
        {
            Console.WriteLine("CreateBucket");
            Console.WriteLine(bucketName);
            try
            {
                if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName) == false)
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    var response = await _client.PutBucketAsync(putBucketRequest);

                    return new S3Response
                    {
                        Message = response.ResponseMetadata.RequestId,
                        Status = response.HttpStatusCode
                    };
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(e);
                return new S3Response
                {
                    Message = e.Message,
                    Status = e.StatusCode
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new S3Response
                {
                    Message = e.Message,
                    Status = HttpStatusCode.InternalServerError
                };
            }
            return new S3Response
            {
                Message = "Algo salio mal",
                Status = HttpStatusCode.InternalServerError
            };
        }

        public async Task<S3Response> GetObjectFromS3Async(string fileName)
        {
            const string bucketName = "pinart-images-storage";

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName
                };

                string responseBody;
                using (var response = await _client.GetObjectAsync(request))
                using (var responseStream = response.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    var title = $"https://pinart-images-storage.s3.amazonaws.com/{fileName}";
                    var contentType = response.Headers["Content-Type"];

                    Console.WriteLine($"Object meta, Title: {title}");
                    Console.WriteLine($"Object meta, contentType: {contentType}");

                    responseBody = reader.ReadToEnd();

                    return new S3Response
                    {
                        Status = HttpStatusCode.Found,
                        Message = title
                    };
                }


            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(e.Message);
                return new S3Response
                {
                    Message = e.Message,
                    Status = e.StatusCode
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new S3Response
                {
                    Message = e.Message,
                    Status = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<S3Response> UploadObject(IFormFile file)
        {
            // get the file and convert it to the byte[]
            byte[] fileBytes = new Byte[file.Length];
            file.OpenReadStream().Read(fileBytes, 0, Int32.Parse(file.Length.ToString()));

            // create unique file name for prevent the mess
            var fileName = Guid.NewGuid() + file.FileName;

            PutObjectResponse response = null;

            using (var stream = new MemoryStream(fileBytes))
            {
                var request = new PutObjectRequest
                {
                    BucketName = "pinart-images-storage",
                    Key = fileName,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                response = await _client.PutObjectAsync(request);
            };

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return new S3Response
                {
                    Status = HttpStatusCode.Created,
                    Message = $"https://pinart-images-storage.s3.amazonaws.com/{fileName}"
                };
            }
            else
            {
                return new S3Response
                {
                    Status = HttpStatusCode.Created,
                    Message = $"https://pinart-images-storage.s3.amazonaws.com/{fileName}"
                };
            }
        }


    }
}