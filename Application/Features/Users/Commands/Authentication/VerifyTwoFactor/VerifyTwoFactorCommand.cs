using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.Authentication.VerifyTwoFactor
{
    public record VerifyTwoFactorCommand(
    Guid UserId,
    string TotpCode,
    string? NewTemporarySecret, 
    string? IpAddress) : IRequest<Result<VerifyTwoFactorResponse>>;
}
