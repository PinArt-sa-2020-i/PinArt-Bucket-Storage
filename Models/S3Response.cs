using System;
using System.Net;

namespace PinArt_Bucket_Storage.Models
{
    public class S3Response
    {
        public HttpStatusCode Status { get; set; }

        public string Message { get; set; }
    }
}