using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("User already exists!");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            
            return new UserDto {
                Username=createdUser.Username,
                Token=CreateToken(createdUser)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(UserForLoginDto userForLoginDto)
        {
            //throw new Exception("Server says no");
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized("Invalid username");

            

            return new UserDto {
                Username=userFromRepo.Username,
                Token=CreateToken(userFromRepo),
                PhotoUrl=userFromRepo.Photos.FirstOrDefault(u=>u.IsMain)?.Url
            };
        }

        public string CreateToken(User user){
            
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.NameId,user.Username)                
            };

            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Token").Value));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor=new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(claims),
                SigningCredentials=creds,
                Expires=DateTime.Now.AddDays(1)
            };

            var tokenHandler=new JwtSecurityTokenHandler();
            var token=tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}