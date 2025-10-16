using Microsoft.Extensions.Logging;

using Moq;

using Student_Loans_eBonder_API.Profile.Services;
using Student_Loans_eBonder_API.Profile.Types.Commands;
using Student_Loans_eBonder_API.Profile.Types.Models;
using Student_Loans_eBonder_API.Tests.Common;

namespace Student_Loans_eBonder_API.Tests.Profile;

public class UserProfileServiceTests : DatabaseTestBase
{
	private Mock<ILogger<UserProfileService>> _mockLogger = null!;
	private UserProfileService _userProfileService = null!;

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();

		_mockLogger = MockLogger<UserProfileService>();
		_userProfileService = new UserProfileService(_mockLogger.Object, TestDbContext);
	}

	[Fact]
	public async Task UserProfileService_WhenCreateUserProfileCalledAndProfileDoesNotExist_ShouldCreateAndPersist()
	{
		// Arrange
		var userId = "user-no-profile";
		var command = new CreateUserProfileCommand { UserId = userId };

		// Act
		var created = await _userProfileService.CreateUserProfile(command);

		// Assert
		Assert.True(created);
		var profile = TestDbContext.UserProfile.SingleOrDefault(p => p.UserId == userId);
		Assert.NotNull(profile);
		Assert.IsType<UserProfile>(profile);
		Assert.NotNull(profile!.Name);
	}

	[Fact]
	public async Task UserProfileService_WhenCreateUserProfileCalledAndProfileAlreadyExists_ShouldReturnFalseAndNotDuplicate()
	{
		// Arrange
		var userId = "user-with-profile";
		TestDbContext.UserProfile.Add(new UserProfile { UserId = userId, Name = new Name() });
		await SaveChangesAsync();

		var command = new CreateUserProfileCommand { UserId = userId };

		// Act
		var created = await _userProfileService.CreateUserProfile(command);

		// Assert
		Assert.False(created);
		var count = TestDbContext.UserProfile.Count(p => p.UserId == userId);
		Assert.Equal(1, count);
	}
}

