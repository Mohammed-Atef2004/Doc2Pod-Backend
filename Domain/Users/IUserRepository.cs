using Domain.Interfaces.Repositories;
using  Domain.Users.ValueObjects;

namespace  Domain.Users;


public interface IUserRepository:IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(Username username, CancellationToken ct = default);
    Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken ct = default);

    Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
    Task<bool> ExistsByUsernameAsync(Username username, CancellationToken ct = default);

    Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken ct = default);

    // ─── Persistence ─────────────────────────────────────────────────────────

    void SoftDelete(User user);   // sets IsDeleted — EF change tracker handles the rest
}  

