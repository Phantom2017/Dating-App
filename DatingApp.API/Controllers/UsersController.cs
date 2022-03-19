using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Authorize]    
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            this.photoService = photoService;
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user=await userRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername=user.Username;
            if(string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender=user.Gender == "male"?"female":"male";

            var members = await userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(members.CurrentPage,members.PageSize
            ,members.TotalCount,members.TotalPages);
            return Ok(members);
        }

        [HttpGet("{username}",Name ="GetUser")]
        public async Task<ActionResult<MemberDto>> GetUsers(string username)
        {
            return await userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

            mapper.Map(memberUpdateDto, user);
            userRepository.Update(user);

            if (await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user.");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

            var result=await photoService.AddPhotoAsync(file);

            if(result.Error!=null) return BadRequest(result.Error.Message);

            var photo=new Photo
            {
                Url=result.SecureUrl.AbsoluteUri,
                PublicId=result.PublicId
            };

            if(user.Photos.Count ==0 ) photo.IsMain=true;

            user.Photos.Add(photo);

            if(await userRepository.SaveAllAsync())
            {
                return CreatedAtRoute("GetUser",new {username=user.Username} ,mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user=await userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo=user.Photos.FirstOrDefault(p=>p.Id==photoId);

            if(photo.IsMain)  return BadRequest("This is already your main photo");

            var currentMain=user.Photos.FirstOrDefault(p=>p.IsMain);
            if(currentMain!=null) currentMain.IsMain=false;
            photo.IsMain=true;

            if(await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user=await userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo=user.Photos.FirstOrDefault(p=>p.Id==photoId);
            if(photo==null) return NotFound();

            if(photo.IsMain) return BadRequest("You can not delete your main photo");

            if(photo.PublicId!=null)
            {
                var result=await photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error!=null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);

            if(await userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete photo");
        }
    }
}