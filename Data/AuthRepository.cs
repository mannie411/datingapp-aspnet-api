using System;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            PasswordHasher<string> pw = new PasswordHasher<string>();

            var user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.Username == username);

            if (user == null)
                return null;

            var verifed = pw.VerifyHashedPassword(user.Username, user.PasswordHash, password);
            if (verifed == PasswordVerificationResult.Failed)
                return null;

            // Auth successfull

            return user;
        }


        public async Task<User> Register(User user, string password)
        {

            PasswordHasher<string> pw = new PasswordHasher<string>();
            user.PasswordHash = pw.HashPassword(user.Username, password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;


        }



        public async Task<bool> UserExist(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))
                return true;

            return false;
        }
    }
}