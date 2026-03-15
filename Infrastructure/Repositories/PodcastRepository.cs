using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Presistence.Data;
using Infrastructure.Repositories.Shared;

namespace Infrastructure.Repositories
{
    internal class PodcastRepository : GenericRepository<Podcast>, IPodcastRepository
    {
        private readonly AppDbContext _context;
        public PodcastRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }

}
