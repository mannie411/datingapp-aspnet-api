using System;

namespace api.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Desc { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isMain { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}