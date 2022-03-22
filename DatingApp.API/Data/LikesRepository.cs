using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext context;
        public LikesRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await context.Likes.FindAsync(sourceUserId,likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users=context.Users.OrderBy(u=>u.Username).AsQueryable();
            var likes=context.Likes.AsQueryable();

            if (likesParams.Predicate=="liked")
            {
                likes=likes.Where(like=>like.SourceUserId==likesParams.UserId);
                users=likes.Select(like=>like.LikedUser);
            }

            if (likesParams.Predicate=="likedBy")
            {
                likes=likes.Where(like=>like.LikedUserId==likesParams.UserId);
                users=likes.Select(like=>like.SourceUser);
            }

            var likedUsers= users.Select(user=> new LikeDto{
                Id=user.Id,
                Username=user.Username,
                PhotoUrl=user.Photos.FirstOrDefault(p=>p.IsMain).Url,
                City=user.City,
                KnownAs=user.KnownAs,
                Age=user.DateOfBirth.CalculateAge()
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers,likesParams.PageNumber,likesParams.PageSize);
        }

        public async Task<User> GetUserWithLikes(int userId)
        {
            return await context.Users
            .Include(u=>u.LikedUsers).FirstOrDefaultAsync(u=>u.Id==userId);
        }
    }
}