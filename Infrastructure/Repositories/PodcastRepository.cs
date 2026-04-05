using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Presistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal class PodcastRepository : GenericRepository<Podcast>, IPodcastRepository
    {
        private readonly AppDbContext _context;
        public PodcastRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Podcast>> GetByUserId(Guid userId)
        {
            return await _context.Podcasts
                .Include(p => p.Document)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }
    }

}
