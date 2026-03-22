using Application.Features.Users.Services;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.SharedKernel;
using Domain.Users;
using Domain.Users.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.Authentication.VerifyTwoFactor
{
    public sealed class VerifyTwoFactorCommandHandler : IRequestHandler<VerifyTwoFactorCommand, Result<VerifyTwoFactorResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITotpService _totpService;
        private readonly ITokenService _tokenService;
        private readonly IAuditService _auditService;

        public VerifyTwoFactorCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ITotpService totpService,
            ITokenService tokenService,
            IAuditService auditService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _totpService = totpService;
            _tokenService = tokenService;
            _auditService = auditService;
        }

        public async Task<Result<VerifyTwoFactorResponse>> Handle(VerifyTwoFactorCommand command, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId);
            if (user is null)
                return Result<VerifyTwoFactorResponse>.Failure(UserErrors.NotFound);

            var availabilityResult = user.CheckAvailability();
            if (availabilityResult.IsFailure)
                return Result<VerifyTwoFactorResponse>.Failure(availabilityResult.Error);

            string? secretToVerify = !string.IsNullOrEmpty(command.NewTemporarySecret)
            ? command.NewTemporarySecret
            : user.TwoFactorSecret;

            if (string.IsNullOrEmpty(secretToVerify))
                return Result<VerifyTwoFactorResponse>.Failure(UserErrors.SecurityErrors.TwoFactorNotEnabled);

            if (!_totpService.Verify(secretToVerify, command.TotpCode))
            {
                user.RecordFailedLogin(command.IpAddress ?? "unknown");
                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync(ct);

                await _auditService.LogAsync(new AuditEntry(
                    ActorId: user.Id,
                    Action: AuditActions.UserLoginFailed,
                    EntityType: nameof(User),
                    EntityId: user.Id,
                    IpAddress: command.IpAddress,
                    Succeeded: false,
                    FailureReason: "Invalid TOTP code provided."), ct);
                return Result<VerifyTwoFactorResponse>.Failure(UserErrors.SecurityErrors.InvalidTotpCode);
            }

            if (!string.IsNullOrEmpty(command.NewTemporarySecret))
            {
                user.CompleteTwoFactorReset(command.NewTemporarySecret);
            }

            var loginResult = user.RecordSuccessfulLogin(command.IpAddress ?? "unknown");
            if (loginResult.IsFailure)
            {
                await _auditService.LogAsync(new AuditEntry(
                    ActorId: user.Id,
                    Action: AuditActions.UserLoginFailed,
                    EntityType: nameof(User),
                    EntityId: user.Id,
                    IpAddress: command.IpAddress,
                    Succeeded: false,
                    FailureReason: loginResult.Error.Message), ct);

                return Result<VerifyTwoFactorResponse>.Failure(loginResult.Error);
            }

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync(ct);

            await _auditService.LogAsync(new AuditEntry(
                ActorId: user.Id,
                Action: AuditActions.UserLogin,
                EntityType: nameof(User),
                EntityId: user.Id,
                IpAddress: command.IpAddress,
                Succeeded: true), ct);

            var claims = new TokenClaims(
                UserId: user.Id,
                Email: user.Email.Value,
                Username: user.Username.Value,
                Role: user.Role.ToString());

            var token = _tokenService.GenerateAccessToken(claims);

            return Result<VerifyTwoFactorResponse>.Success(new VerifyTwoFactorResponse(
                Token: token,
                UserId: user.Id,
                Username: user.Username.Value,
                Email: user.Email.Value,
                Role: user.Role.ToString()
            ));
        }
    }
}
