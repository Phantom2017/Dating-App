using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;

        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await context.Users.Where(u => u.Username == username).
            ProjectTo<MemberDto>(mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query=context.Users.AsQueryable();

            query=query.Where(u=>u.Username!=userParams.CurrentUsername );
            query=query.Where(u=>u.Gender==userParams.Gender);
            
            var minDob=DateTime.Today.AddYears(-userParams.MaxAge-1);
            var maxDob=DateTime.Today.AddYears(-userParams.MinAge);
            query=query.Where(u=>u.DateOfBirth>=minDob && u.DateOfBirth<=maxDob);

            switch (userParams.OrderBy)
            {
                case "created":
                    query=query.OrderByDescending(u=>u.Created);
                    break;
                
                default:
                    query=query.OrderByDescending(u=>u.LastActive);
                    break;
            }
                    
            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            .AsNoTracking(),userParams.PageNumber,userParams.PageSize);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await context.Users.Include(u => u.Photos).SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await context.Users.Include(u => u.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void Update(User user)
        {
            context.Entry(user).State = EntityState.Modified;
        }
    }
}