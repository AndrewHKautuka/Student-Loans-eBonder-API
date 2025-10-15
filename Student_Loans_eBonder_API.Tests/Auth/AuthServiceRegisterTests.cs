using Microsoft.Extensions.Logging;

using Moq;

using Student_Loans_eBonder_API.Auth.Services;
using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Tests.Common;

namespace Student_Loans_eBonder_API.Tests.Auth;

/// <summary>
/// Unit tests for AuthService class
/// </summary>
public class AuthServiceTests : DatabaseTestBase
{
	private Mock<ILogger<AuthService>> _mockLogger = null!;
	private AuthService _authService = null!;

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync().ConfigureAwait(true);

		_mockLogger = MockLogger<AuthService>();

		_authService = new AuthService(_mockLogger.Object, UserManager);
	}

	[Fact]
	public async Task AuthService_WhenRegisterCalledWithValidCommand_ShouldReturnSuccessResult()
	{
		// Arrange
		var command = new RegisterUserCommand
		{
			Email = "test@example.com",
			Password = "TestPassword123!"
		};

		// Act
		var (result, userId) = await _authService.Register(command);

		// Assert
		Assert.True(result.Succeeded);
		Assert.NotNull(userId);
	}

	[Fact]
	public async Task AuthService_WhenRegisterCalledWithValidCommand_ShouldMapEmailToUserName()
	{
		// Arrange
		var command = new RegisterUserCommand
		{
			Email = "test@example.com",
			Password = "TestPassword123!"
		};

		// Act
		var (result, userId) = await _authService.Register(command);

		// Assert
		Assert.True(result.Succeeded);
		Assert.NotNull(userId);
		var createdUser = TestDbContext.Users.Single(u => u.Email == command.Email);
		Assert.Equal(command.Email, createdUser.UserName);
	}

	[Fact]
	public async Task AuthService_WhenRegisterCalledWithInvalidEmail_ShouldReturnFailureResult()
	{
		// Arrange
		var command = new RegisterUserCommand
		{
			Email = "invalid-email",
			Password = "TestPassword123!"
		};

		// Act
		var (result, userId) = await _authService.Register(command);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Null(userId);
		Assert.Contains(result.Errors, e => e.Code == "InvalidEmail");
	}

	[Fact]
	public async Task AuthService_WhenRegisterCalledWithTooShortPassword_ShouldReturnFailureResult()
	{
		// Arrange
		var command = new RegisterUserCommand
		{
			Email = "test@example.com",
			Password = "123"
		};

		// Act
		var (result, userId) = await _authService.Register(command);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Null(userId);
		Assert.Contains(result.Errors, e => e.Code == "PasswordTooShort");
	}

	[Fact]
	public async Task AuthService_WhenRegisterCalledWithDuplicateEmail_ShouldReturnFailureResult()
	{
		// Arrange
		var command = new RegisterUserCommand
		{
			Email = "test@example.com",
			Password = "TestPassword123!"
		};

		// First registration should succeed
		var (firstResult, firstUserId) = await _authService.Register(command);
		Assert.True(firstResult.Succeeded);
		Assert.NotNull(firstUserId);

		// Second registration with same email should fail
		var (secondResult, secondUserId) = await _authService.Register(command);
		Assert.False(secondResult.Succeeded);
		Assert.Null(secondUserId);
		Assert.Contains(secondResult.Errors, e => e.Code == "DuplicateEmail");
	}

	[Fact]
	public async Task AuthService_WhenRegisterCalledWithNullCommand_ShouldThrowArgumentNullException()
	{
		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => _authService.Register(null!));
	}
}
