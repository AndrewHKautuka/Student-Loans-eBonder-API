using Mapster;

using Microsoft.AspNetCore.Identity;

using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Auth.Types.Requests;

namespace Student_Loans_eBonder_API.Auth.Services;

internal partial class AuthService
{
	private readonly ILogger<AuthService> _logger;
	private readonly UserManager<User> _userManager;

	public AuthService(ILogger<AuthService> logger, UserManager<User> userManager)
	{
		_logger = logger;
		_userManager = userManager;
	}

	public async Task<IdentityResult> Register(RegisterUserCommand registerUserCommand)
	{
		LogRegisterAttemptMessage(_logger, registerUserCommand.Email);

		TypeAdapterConfig.GlobalSettings.NewConfig<RegisterUserCommand, User>().Map(dest => dest.UserName, src => src.Email);

		var (user, password) = registerUserCommand.Adapt<(User User, string Password)>();

		var result = await _userManager.CreateAsync(user, password).ConfigureAwait(false);
		LogRegisterStatusMessage(_logger, result.Succeeded ? "Successfully registered" : "Failed to register", registerUserCommand.Email);

		return result;
	}

	public async Task<IdentityResult> RegisterStudent(RegisterStudentRequest registerStudentRequest)
	{
		return await Register(registerStudentRequest.Adapt<RegisterUserCommand>()).ConfigureAwait(true);
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Attempt to register {Email}")]
	static partial void LogRegisterAttemptMessage(ILogger logger, string email);

	[LoggerMessage(Level = LogLevel.Information, Message = "{RegistrationStatus} new User with email {Email}")]
	static partial void LogRegisterStatusMessage(ILogger logger, string registrationStatus, string email);
}
