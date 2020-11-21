using System.Collections.Generic;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace api.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;

        }

        public void SeedUsers()
        {
            var usersData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var result = JsonConvert.DeserializeObject<List<User>>(usersData);

            foreach (var user in result)
            {

                PasswordHasher<string> pw = new PasswordHasher<string>();
                user.PasswordHash = pw.HashPassword(user.Username, "password");
                user.Username = user.Username.ToLower();
                _context.Users.Add(user);
            }

            _context.SaveChanges();

        }
    }
}