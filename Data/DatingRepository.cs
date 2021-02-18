using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);

        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;

        }

        public async Task<Photo> GetMainPhoto(int userId)
        {
            var photo = await _context.Photos.Where(u => u.UserId == userId)
                                 .FirstOrDefaultAsync(p => p.isMain);

            return photo;

        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(f => f.Photos)
                                .SingleOrDefaultAsync(u => u.Id == id);
            return user;

        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(f => f.Photos).AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId)
                            .Where(g => g.Gender == userParams.Gender);

            if (userParams.Likees)
            {
                var likees = await GetUserLikes(userParams.UserId, userParams.Likees);
                // users = users.Where(u => likees.Contains(u.Id));

            }

            if (userParams.Likers)
            {
                var likers = await GetUserLikes(userParams.UserId, userParams.Likers);
                // users = users.Where(u => likers.Contains(u.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DOB >= minDob && u.DOB <= maxDob);
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            var like = await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
            return like;
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likes)
        {
            var user = await _context.Users
                                .Include(u => u.Likee)
                                .Include(u => u.Liker)
                                .FirstOrDefaultAsync(u => u.Id == id);
            if (likes) nb 
            {
                var likee = user.Likee.Where(u => u.LikeeId == id).Select(i => i.LikerId);
                return likee;

            }
            else
            {
                var liker = user.Liker.Where(u => u.LikerId == id).Select(i => i.LikeeId);
                return liker;

            }
        }
    }
}