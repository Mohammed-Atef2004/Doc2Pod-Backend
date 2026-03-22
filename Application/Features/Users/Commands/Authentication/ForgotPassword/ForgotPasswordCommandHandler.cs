using Application.Features.Users.Services;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Settings;
using Domain.SharedKernel;
using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.Authentication.ForgotPassword
{
    public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly ApiSettings _apiSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IIdentityService identityService,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            IOptions<ApiSettings> apiOptions,
            IHttpContextAccessor httpContextAccessor,
            IAuditService auditService)
        {
            _userRepository = userRepository;
            _identityService = identityService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _apiSettings = apiOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            _auditService = auditService;
        }

        public async Task<Result> Handle(ForgotPasswordCommand command, CancellationToken ct)
        {
            var emailResult = Email.Create(command.Email);
            if (emailResult.IsFailure)
            {
                return Result.Success();
            }
            var user = await _userRepository.GetByEmailAsync(emailResult.Value, ct);
            if (user is null)
            {
                return Result.Success();
            }

            var availabilityResult = user.CheckAvailability();
            if (availabilityResult.IsFailure)
                return availabilityResult;

            var token = await _identityService.GeneratePasswordResetTokenAsync(user.Email.Value, ct);

            if (string.IsNullOrEmpty(token))
            {
                await _auditService.LogAsync(new AuditEntry(
                    ActorId: user.Id,
                    Action: AuditActions.UserForgotPassword,
                    EntityType: "User",
                    EntityId: user.Id,
                    IpAddress: _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                    Succeeded: false,
                    FailureReason: "Failed to generate identity token."
                ), ct);
                await _unitOfWork.CompleteAsync(ct);
                return Result.Success();
            }
            var resetLink = $"{_apiSettings.BaseUrl}/api/auth/reset-password?" +
                $"userId={user.Id}&" +
                $"token={WebUtility.UrlEncode(token)}";
            var emailTask = await _emailService.SendPasswordResetEmailAsync(
                user.Email.Value,
                user.FullName.DisplayName,
                resetLink, 
                ct);

            await _auditService.LogAsync(new AuditEntry(
            ActorId: user.Id,
            Action: AuditActions.UserForgotPassword,
            EntityType: "User",
            EntityId: user.Id,
            IpAddress: _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            Succeeded: emailTask.IsSuccess,
            FailureReason: emailTask.IsFailure ? emailTask.Error.Message : null
            ), ct);
            await _unitOfWork.CompleteAsync(ct);

            if (emailTask.IsFailure)
            {
                return Result.Failure(emailTask.Error);
            }
            return Result.Success();
        }
    }
}
