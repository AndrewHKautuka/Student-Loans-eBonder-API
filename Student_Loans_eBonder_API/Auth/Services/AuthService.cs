using Mapster;

using Microsoft.AspNetCore.Identity;

using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Auth.Types.Requests;
using Student_Loans_eBonder_API.Profile.Services;
using Student_Loans_eBonder_API.Profile.Types.Commands;

namespace Student_Loans_eBonder_API.Auth.Services;

public partial class AuthService
{
	private readonly ILogger<AuthService> _logger;
	private readonly UserManager<User> _userManager;
	private readonly UserProfileService _userProfileService;

	public AuthService(ILogger<AuthService> logger, UserManager<User> userManager, UserProfileService userProfileService)
	{
		_logger = logger;
		_userManager = userManager;
		_userProfileService = userProfileService;
	}

	public async Task<(IdentityResult, string? UserId)> Register(RegisterUserCommand registerUserCommand)
	{
		ArgumentNullException.ThrowIfNull(registerUserCommand);

		LogRegisterAttemptMessage(_logger, registerUserCommand.Email);

		TypeAdapterConfig.GlobalSettings.NewConfig<RegisterUserCommand, User>().Map(dest => dest.UserName, src => src.Email);

		var (user, password) = registerUserCommand.Adapt<(User User, string Password)>();

		var result = await _userManager.CreateAsync(user, password);

		if (result.Succeeded)
		{
			var userId = user.Id;
			var profileCreated = await _userProfileService.CreateUserProfile(new CreateUserProfileCommand { UserId = userId });

			if (!profileCreated)
			{
				await _userManager.DeleteAsync(user);
				LogRegisterFailedMessage(_logger, registerUserCommand.Email);
				return (IdentityResult.Failed(new IdentityError() { Description = "Failed to create corresponding user profile" }), null);
			}

			LogRegisterSuccessfulMessage(_logger, registerUserCommand.Email);
			return (result, userId);
		}
		else
		{
			LogRegisterFailedMessage(_logger, registerUserCommand.Email);
			return (result, null);
		}

	}

	public async Task<(IdentityResult, string? UserId)> RegisterStudent(RegisterStudentRequest registerStudentRequest)
	{
		ArgumentNullException.ThrowIfNull(registerStudentRequest);
		return await Register(registerStudentRequest.Adapt<RegisterUserCommand>());
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Attempt to register {Email}")]
	static partial void LogRegisterAttemptMessage(ILogger logger, string email);

	[LoggerMessage(Level = LogLevel.Information, Message = "Successfully registered new User with email {Email}")]
	static partial void LogRegisterSuccessfulMessage(ILogger logger, string email);

	[LoggerMessage(Level = LogLevel.Information, Message = "Failed to register new User with email {Email}")]
	static partial void LogRegisterFailedMessage(ILogger logger, string email);
}
