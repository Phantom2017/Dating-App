using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper mapper;
        private readonly DataContext context;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IUserRepository UserRepository => new UserRepository(context,mapper);

        public ILikesRepository LikesRepository => new LikesRepository(context);

        public IMessageRepository MessageRepository => new MessageRepository(context,mapper);

        public IPhotoRepository PhotoRepository => new PhotoRepository(context);

        public async Task<bool> Complete()
        {
            return await context.SaveChangesAsync()>0;
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}