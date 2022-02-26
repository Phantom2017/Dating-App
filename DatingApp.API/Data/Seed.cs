using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if(await context.Users.AnyAsync()) return;

            var userData=await File.ReadAllTextAsync("Data/UserSeedData.json");
            var users=JsonSerializer.Deserialize<List<User>>(userData);

            foreach (var user in users)
            {
                using (var hmac=new HMACSHA512())
                {
                    user.Username=user.Username.ToLower();
                    user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                    user.PasswordSalt=hmac.Key;

                    context.Users.Add(user);
                } 
            }

            await context.SaveChangesAsync();
        }
    }
}