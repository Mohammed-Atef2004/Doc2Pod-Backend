using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.Security.Enable2FA
{
    public sealed record Enable2FACommand(
    Guid UserId,
    string TotpCode
) : IRequest<Result<Enable2FAResponse>>;
}
