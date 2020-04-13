using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PinArt_Bucket_Storage.Models;

namespace PinArt_Bucket_Storage.Services
{
    public interface IS3Service
    {
        Task<S3Response> CreateBucketAsync(string bucketName);
        Task<S3Response> UploadObject(IFormFile file);
        Task<S3Response> GetObjectFromS3Async(string fileName);

        Task<S3Response> DeleteObjectFromS3Async(string keyName);
    }
}