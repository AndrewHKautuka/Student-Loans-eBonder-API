using Mapster;

using Microsoft.AspNetCore.Identity;

using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Auth.Types.Requests;
using Student_Loans_eBonder_API.Profile.Services;
using Student_Loans_eBonder_API.Profile.Types.Commands;
using Student_Loans_eBonder_API.Student.Services;
using Student_Loans_eBonder_API.Student.Types.Commands;

namespace Student_Loans_eBonder_API.Auth.Services;

public partial class AuthService(ILogger<AuthService> logger, UserManager<User> userManager, UserProfileService userProfileService, StudentService studentService)
{
	public async Task<(IdentityResult, string? UserId)> Register(RegisterUserCommand registerUserCommand)
	{
		ArgumentNullException.ThrowIfNull(registerUserCommand);

		LogRegisterAttemptMessage(logger, registerUserCommand.Email);

		TypeAdapterConfig.GlobalSettings.NewConfig<RegisterUserCommand, User>().Map(dest => dest.UserName, src => src.Email);

		var (user, password) = registerUserCommand.Adapt<(User User, string Password)>();

		var result = await userManager.CreateAsync(user, password);

		if (result.Succeeded)
		{
			var userId = user.Id;
			var profileCreated = await userProfileService.CreateUserProfile(new CreateUserProfileCommand { UserId = userId });

			if (!profileCreated)
			{
				await userManager.DeleteAsync(user);
				LogRegisterFailedMessage(logger, registerUserCommand.Email);
				return (IdentityResult.Failed(new IdentityError() { Description = "Failed to create corresponding user profile" }), null);
			}

			LogRegisterSuccessfulMessage(logger, registerUserCommand.Email);
			return (result, userId);
		}
		else
		{
			LogRegisterFailedMessage(logger, registerUserCommand.Email);
			return (result, null);
		}

	}

	public async Task<(IdentityResult, string? UserId)> RegisterStudent(RegisterStudentRequest registerStudentRequest)
	{
		ArgumentNullException.ThrowIfNull(registerStudentRequest);
		var (result, userId) = await Register(registerStudentRequest.Adapt<RegisterUserCommand>());

		if (result.Succeeded)
		{
			var studentCreated = await studentService.CreateStudent(new CreateStudentCommand() { UserId = userId! });

			if (!studentCreated)
			{
				var user = await userManager.FindByIdAsync(userId!);
				await userManager.DeleteAsync(user!);
				LogRegisterUnrolledMessage(logger, registerStudentRequest.Email);
				return (IdentityResult.Failed(new IdentityError() { Description = "Failed to create corresponding student record" }), null);
			}
		}

		return (result, userId);
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Attempt to register {Email}")]
	static partial void LogRegisterAttemptMessage(ILogger logger, string email);

	[LoggerMessage(Level = LogLevel.Information, Message = "Successfully registered new User with email {Email}")]
	static partial void LogRegisterSuccessfulMessage(ILogger logger, string email);

	[LoggerMessage(Level = LogLevel.Information, Message = "Failed to register new User with email {Email}")]
	static partial void LogRegisterFailedMessage(ILogger logger, string email);

	[LoggerMessage(Level = LogLevel.Information, Message = "Unrolled registeration of new User with email {Email}")]
	static partial void LogRegisterUnrolledMessage(ILogger logger, string email);
}
