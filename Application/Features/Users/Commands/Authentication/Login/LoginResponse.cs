using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.Authentication.VerifyTwoFactor
{
    public record LoginResponse(
        string? Token,
        Guid UserId,
        bool RequiresTwoFactor
    );
}

