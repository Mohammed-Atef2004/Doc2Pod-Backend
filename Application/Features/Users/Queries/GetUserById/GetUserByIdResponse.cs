using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries.GetUserById
{
    public record GetUserByIdResponse(
        Guid Id,
        string Username,
        string Email,
        string Role,
        bool IsActive,
        bool IsEmailConfirmed,
        DateTime? LastLogin);
}
