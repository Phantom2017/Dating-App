using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Dtos;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {        
        private readonly ILikesRepository likesRepository;
        private readonly IUserRepository userRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            this.userRepository = userRepository;
            this.likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId=User.GetUserId();
            var likedUser=await userRepository.GetUserByUsernameAsync(username);
            var sourceUser=await likesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser==null) return NotFound();

            if(sourceUser.Username==username) return BadRequest("You cannot like yourself!");

            var userLike=await likesRepository.GetUserLike(sourceUserId,likedUser.Id);
            if(userLike!=null) return BadRequest("You already like this user");

            sourceUser.LikedUsers.Add(new UserLike
            {
                SourceUserId=sourceUserId,
                LikedUserId=likedUser.Id
            });

           if( await userRepository.SaveAllAsync())  return Ok();

           return BadRequest("Failed to like user");
        } 

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId=User.GetUserId();
            var users= await likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);
            return Ok(users);
        }
    }
}