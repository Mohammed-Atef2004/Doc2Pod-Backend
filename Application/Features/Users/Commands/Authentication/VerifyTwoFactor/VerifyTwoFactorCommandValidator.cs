using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.Authentication.VerifyTwoFactor
{
    public sealed class VerifyTwoFactorCommandValidator : AbstractValidator<VerifyTwoFactorCommand>
    {
        public VerifyTwoFactorCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.TotpCode)
                .NotEmpty().WithMessage("OTP code is required.")
                .Length(6).WithMessage("OTP code must be exactly 6 digits.")
                .Matches(@"^[0-9]*$").WithMessage("OTP code must contain only numbers.");

            RuleFor(x => x.IpAddress)
                .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.IpAddress));
        }
    }
}
