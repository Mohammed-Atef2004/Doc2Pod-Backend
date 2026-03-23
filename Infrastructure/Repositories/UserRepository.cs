using Domain.Users;
using Domain.Users.ValueObjects;
using Infrastructure.Presistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default)
        {
            var emailValue = email.Value;
            return await _context.Users
                .FromSqlRaw("SELECT * FROM DomainUsers WHERE Email = {0}", emailValue)
                .AnyAsync(ct);
        }

        public async Task<bool> ExistsByUsernameAsync(Username username, CancellationToken ct = default)
        {
            var usernameValue = username.Value;
            return await _context.Users
                .FromSqlRaw("SELECT * FROM DomainUsers WHERE Username = {0}", usernameValue)
                .AnyAsync(ct);
        }

        public Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
        {
            var emailValue = email.Value;
            return _context.Users
                .FromSqlRaw("SELECT * FROM DomainUsers WHERE Email = {0}", emailValue)
                .FirstOrDefaultAsync(ct);
        }

        public Task<User?> GetByUsernameAsync(Username username, CancellationToken ct = default)
        {
            var usernameValue = username.Value;
            return _context.Users
                .FromSqlRaw("SELECT * FROM DomainUsers WHERE Username = {0}", usernameValue)
                .FirstOrDefaultAsync(ct);
        }
    }
}