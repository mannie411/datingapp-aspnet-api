using System;
using Microsoft.AspNetCore.Http;

namespace api.Dtos
{
    public class PhotoForUploadDto
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Desc { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PublicId { get; set; }

        public PhotoForUploadDto()
        {
            CreatedAt = DateTime.Now;

        }
    }
}