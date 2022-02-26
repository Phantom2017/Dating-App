using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Dtos;
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

        Task<IEnumerable<MemberDto>> GetMembersAsync();
        Task<MemberDto> GetMemberAsync(string username);
    }    
}