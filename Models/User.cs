using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastActive { get; set; }
        public string KnownAs { get; set; }
        public string Introduction { get; set; }
        public string Interest { get; set; }
        public string LookingFor { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Like> Liker { get; set; }
        public ICollection<Like> Likee { get; set; }

        public User()
        {
            Photos = new Collection<Photo>();
        }

    }
}