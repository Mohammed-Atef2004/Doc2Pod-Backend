using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Presistence.Data;
using Infrastructure.Repositories.Shared;

namespace Infrastructure.Repositories
{
    public class DocumentRepository :
        GenericRepository<Document>, IDocumentRepository
    {
        private readonly AppDbContext _context;
        public DocumentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }

}
