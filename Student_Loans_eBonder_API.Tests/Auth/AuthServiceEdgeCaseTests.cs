using Microsoft.Extensions.Logging;

using Moq;

using Student_Loans_eBonder_API.Auth.Services;
using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Auth.Types.Requests;
using Student_Loans_eBonder_API.Profile.Services;
using Student_Loans_eBonder_API.Tests.Common;

namespace Student_Loans_eBonder_API.Tests.Auth;

public class AuthServiceEdgeCaseTests : DatabaseTestBase
{
	private Mock<ILogger<AuthService>> _mockLogger = null!;
	private AuthService _authService = null!;

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();

		_mockLogger = MockLogger<AuthService>();

		var mockProfileLogger = MockLogger<UserProfileService>();
		var userProfileService = new UserProfileService(mockProfileLogger.Object, TestDbContext);
		_authService = new AuthService(_mockLogger.Object, UserManager, userProfileService);
	}

	[Fact]
	public async Task AuthService_WhenRegisterCalledWithMultipleErrors_ShouldReturnAllErrors()
	{
		// Arrange
		var command = new RegisterUserCommand
		{
			Email = "invalid",
			Password = "abc"
		};

		// Act
		var (result, userId) = await _authService.Register(command);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Null(userId);
		Assert.Contains(result.Errors, e => e.Code == "PasswordTooShort");
		Assert.Contains(result.Errors, e => e.Code == "PasswordRequiresDigit");
	}

	[Fact]
	public async Task AuthService_WhenRegisterStudentCalledWithMultipleErrors_ShouldReturnAllErrors()
	{
		// Arrange
		var request = new RegisterStudentRequest
		{
			Email = "invalid",
			Password = "abc"
		};

		// Act
		var (result, userId) = await _authService.RegisterStudent(request);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Null(userId);
		Assert.Contains(result.Errors, e => e.Code == "PasswordTooShort");
		Assert.Contains(result.Errors, e => e.Code == "PasswordRequiresDigit");
	}
}
