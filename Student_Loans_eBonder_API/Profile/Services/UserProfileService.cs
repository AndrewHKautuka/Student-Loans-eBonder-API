using Mapster;

using Microsoft.EntityFrameworkCore;

using Student_Loans_eBonder_API.Profile.Types.Commands;
using Student_Loans_eBonder_API.Profile.Types.Models;

namespace Student_Loans_eBonder_API.Profile.Services;

public partial class UserProfileService(ILogger<UserProfileService> logger, ApplicationDBContext dBContext)
{
	public async Task<bool> CreateUserProfile(CreateUserProfileCommand command)
	{
		LogCheckProfileExistanceMessage(logger);

		var existingProfile = await dBContext.UserProfile.FirstOrDefaultAsync(x => x.UserId == command.UserId);

		if (existingProfile is not null)
		{
			LogProfileAlreadyExistMessage(logger);
			return false;
		}

		LogProfileDoesNotExistMessage(logger);

		LogCreateNewProfileMessage(logger);
		var userProfile = command.Adapt<UserProfile>();

		LogCreateNewProfileMessage(logger);
		await dBContext.UserProfile.AddAsync(userProfile);
		await dBContext.SaveChangesAsync();

		return true;
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Checking if user already has a profile")]
	static partial void LogCheckProfileExistanceMessage(ILogger logger);

	[LoggerMessage(Level = LogLevel.Information, Message = "User already has a profile, no new profile to be created")]
	static partial void LogProfileAlreadyExistMessage(ILogger logger);

	[LoggerMessage(Level = LogLevel.Information, Message = "User does not have a profile, a new profile shall be created")]
	static partial void LogProfileDoesNotExistMessage(ILogger logger);

	[LoggerMessage(Level = LogLevel.Information, Message = "Creating new user profile")]
	static partial void LogCreateNewProfileMessage(ILogger logger);

	[LoggerMessage(Level = LogLevel.Information, Message = "Adding new user profile record")]
	static partial void LogAddNewProfileMessage(ILogger logger);
}
