using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options){ }

        
        public DbSet<User> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLike>().HasKey(k=>new {k.SourceUserId,k.LikedUserId});

            modelBuilder.Entity<UserLike>()
            .HasOne(s=> s.SourceUser)
            .WithMany(l=>l.LikedUsers)
            .HasForeignKey(f=>f.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLike>()
            .HasOne(s=> s.LikedUser)
            .WithMany(l=>l.LikedByUsers)
            .HasForeignKey(f=>f.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}