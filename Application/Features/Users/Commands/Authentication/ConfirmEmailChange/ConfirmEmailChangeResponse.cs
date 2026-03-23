using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.Authentication.ConfirmEmailChange
{
    public record ConfirmEmailChangeResponse(
        string Token,
        string Message);
}
