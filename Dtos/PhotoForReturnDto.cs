using System;

namespace api.Dtos
{
    public class PhotoForReturnDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Desc { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isMain { get; set; }
        public string PublicId { get; set; }
    }
}