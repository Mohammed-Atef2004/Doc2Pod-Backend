using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IPodcastRepository : IGenericRepository<Podcast>
    {
        public Task<List<Podcast>> GetByUserId(Guid userId);
    }
}
