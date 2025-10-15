using Microsoft.Extensions.Logging;

using Moq;

using Student_Loans_eBonder_API.Auth.Services;
using Student_Loans_eBonder_API.Auth.Types.Requests;
using Student_Loans_eBonder_API.Tests.Common;

namespace Student_Loans_eBonder_API.Tests.Auth;

/// <summary>
/// Unit tests for AuthService class
/// </summary>
public class AuthServiceStudentTests : DatabaseTestBase
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
	public async Task AuthService_WhenRegisterStudentCalledWithValidRequest_ShouldReturnSuccessResult()
	{
		// Arrange
		var request = new RegisterStudentRequest
		{
			Email = "test@example.com",
			Password = "TestPassword123!"
		};

		// Act
		var (result, userId) = await _authService.RegisterStudent(request);

		// Assert
		Assert.True(result.Succeeded);
		Assert.NotNull(userId);
	}

	[Fact]
	public async Task AuthService_WhenRegisterCalledWithValidRequest_ShouldMapEmailToUserName()
	{
		// Arrange
		var request = new RegisterStudentRequest
		{
			Email = "test@example.com",
			Password = "TestPassword123!"
		};

		// Act
		var (result, userId) = await _authService.RegisterStudent(request);

		// Assert
		Assert.True(result.Succeeded);
		Assert.NotNull(userId);
		var createdUser = TestDbContext.Users.Single(u => u.Email == request.Email);
		Assert.Equal(request.Email, createdUser.UserName);
	}

	[Fact]
	public async Task AuthService_WhenRegisterStudentCalledWithInvalidEmail_ShouldReturnFailureResult()
	{
		// Arrange
		var request = new RegisterStudentRequest
		{
			Email = "invalid-email",
			Password = "TestPassword123!"
		};

		// Act
		var (result, userId) = await _authService.RegisterStudent(request);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Null(userId);
		Assert.Contains(result.Errors, e => e.Code == "InvalidEmail");
	}

	[Fact]
	public async Task AuthService_WhenRegisterStudentCalledWithTooShortPassword_ShouldReturnFailureResult()
	{
		// Arrange
		var request = new RegisterStudentRequest
		{
			Email = "test@example.com",
			Password = "123"
		};

		// Act
		var (result, userId) = await _authService.RegisterStudent(request);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Null(userId);
		Assert.Contains(result.Errors, e => e.Code == "PasswordTooShort");
	}

	[Fact]
	public async Task AuthService_WhenRegisterStudentCalledWithDuplicateEmail_ShouldReturnFailureResult()
	{
		// Arrange
		var request = new RegisterStudentRequest
		{
			Email = "test@example.com",
			Password = "TestPassword123!"
		};

		// First registration should succeed
		var (firstResult, firstUserId) = await _authService.RegisterStudent(request);
		Assert.True(firstResult.Succeeded);
		Assert.NotNull(firstUserId);

		// Second registration with same email should fail
		var (secondResult, secondUserId) = await _authService.RegisterStudent(request);
		Assert.False(secondResult.Succeeded);
		Assert.Null(secondUserId);
		Assert.Contains(secondResult.Errors, e => e.Code == "DuplicateEmail");
	}

	[Fact]
	public async Task AuthService_WhenRegisterStudentCalledWithNullRequest_ShouldThrowArgumentNullException()
	{
		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => _authService.RegisterStudent(null!));
	}
}
