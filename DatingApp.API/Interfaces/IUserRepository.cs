using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int id);        
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> SaveAllAsync();
        void Update(User user);
        Task<IEnumerable<User>> GetUsersAsync();

        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberAsync(string username);
    }    
}