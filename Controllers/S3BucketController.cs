using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PinArt_Bucket_Storage.Models;
using PinArt_Bucket_Storage.Services;

namespace PinArt_Bucket_Storage.Controllers
{
    [Route("api/S3Bucket")]
    [ApiController]
    public class S3BucketController : Controller
    {
        private readonly IS3Service _service;

        public S3BucketController(IS3Service service)
        {
            _service = service;
        }

        [HttpPost("{bucketName}")]
        public async Task<IActionResult> createBucket([FromRoute] string bucketName)
        {
            Console.WriteLine(bucketName);
            var response = await _service.CreateBucketAsync(bucketName);
            return Ok(response);
        }

        [HttpPost]
        [Route("AddFile")]
        public async Task<JsonResult> AddFile()
        {
            var file = this.Request.Form.Files[0];

            if (!file.ContentType.Contains("image"))
            {
                return new JsonResult(
                    new S3Response
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Tiene que ser una imagen!"
                    }
                );
            }

            var imageResponse = await _service.UploadObject(file);
            JsonResult response = new JsonResult(imageResponse);
            return response;
        }

        [HttpGet]
        [Route("GetFile/{fileName}")]
        public async Task<S3Response> GetObjectFromS3Async([FromRoute] string fileName)
        {
            var response = await _service.GetObjectFromS3Async(fileName);
            return response;
        }

    }
}