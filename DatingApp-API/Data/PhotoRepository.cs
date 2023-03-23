using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext dataContext;
        public PhotoRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<Photo> GetPhotoById(int id)
        {
            return await dataContext.Photos.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await dataContext.Photos.IgnoreQueryFilters()
            .Where(p=>p.IsApproved==false)
            .Select(u=>new PhotoForApprovalDto{
                Id=u.Id,
                Username=u.AppUser.UserName,
                Url=u.Url,
                IsApproved=u.IsApproved
            }).ToListAsync();
        }

        public void RemovePhoto(Photo photo)
        {
            dataContext.Photos.Remove(photo);
        }
    }
}