using System.IdentityModel.Tokens.Jwt;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using Student_Loans_eBonder_API.Auth.Services;
using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Auth.Types.Responses;
using Student_Loans_eBonder_API.Common;

namespace Student_Loans_eBonder_API.Tests.Auth;

public class TokenServiceTests : IAsyncLifetime
{
	private readonly Mock<ILogger<TokenService>> _mockLogger;
	private readonly Mock<IConfiguration> _mockConfiguration;
	private readonly TokenService _tokenService;

	public TokenServiceTests()
	{
		_mockLogger = new Mock<ILogger<TokenService>>();
		_mockConfiguration = new Mock<IConfiguration>();

		// Setup default JWT key for testing
		_mockConfiguration.Setup(x => x["JWTKey"])
			.Returns("ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456789");

		_tokenService = new TokenService(_mockLogger.Object, _mockConfiguration.Object);
	}

	public Task InitializeAsync()
	{
		MapsterConfig.SetupMapsterAdapterConfig();
		return Task.CompletedTask;
	}

	public Task DisposeAsync()
	{
		return Task.CompletedTask;
	}

	[Fact]
	public void TokenService_WhenBuildAccessTokenCalled_ShouldReturnValidBuildAccessTokenResponse()
	{
		// Arrange
		var userId = "test-user-123";
		var command = new BuildAccessTokenCommand { UserId = userId };

		// Act
		var result = _tokenService.BuildAccessToken(command);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Token);
		Assert.NotEmpty(result.Token);
		Assert.True(result.Expires > DateTime.UtcNow);
		Assert.True(result.Expires <= DateTime.UtcNow.AddMonths(1).AddMinutes(1)); // Allow 1 minute tolerance

		// Verify the token can be parsed and contains expected claims
		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(result.Token);

		Assert.Equal(userId, token.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
		Assert.True(token.ValidTo > DateTime.UtcNow);
	}

	[Fact]
	public void TokenService_WhenBuildAccessTokenCalled_ShouldReturnTokenWithCorrectExpiration()
	{
		// Arrange
		var userId = "test-user-789";
		var command = new BuildAccessTokenCommand { UserId = userId };
		var expectedExpiration = DateTime.UtcNow.AddMonths(1);

		// Act
		var result = _tokenService.BuildAccessToken(command);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.Expires >= expectedExpiration.AddMinutes(-1)); // Allow 1 minute tolerance
		Assert.True(result.Expires <= expectedExpiration.AddMinutes(1)); // Allow 1 minute tolerance
	}

	[Fact]
	public void TokenService_WhenBuildAccessTokenCalledWithDifferentUsers_ShouldReturnDifferentTokens()
	{
		// Arrange
		var userId1 = "user-1";
		var userId2 = "user-2";
		var command1 = new BuildAccessTokenCommand { UserId = userId1 };
		var command2 = new BuildAccessTokenCommand { UserId = userId2 };

		// Act
		var result1 = _tokenService.BuildAccessToken(command1);
		var result2 = _tokenService.BuildAccessToken(command2);

		// Assert
		Assert.NotEqual(result1.Token, result2.Token);

		// Verify different user IDs in tokens
		var handler = new JwtSecurityTokenHandler();
		var token1 = handler.ReadJwtToken(result1.Token);
		var token2 = handler.ReadJwtToken(result2.Token);

		Assert.Equal(userId1, token1.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
		Assert.Equal(userId2, token2.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
	}

	[Fact]
	public async Task TokenService_WhenBuildAccessTokenCalledMultipleTimes_ShouldReturnValidTokens()
	{
		// Arrange
		var userId = "test-user-multiple";
		var command = new BuildAccessTokenCommand { UserId = userId };

		// Act
		var results = new List<BuildAccessTokenResponse>();
		for (int i = 0; i < 5; i++)
		{
			results.Add(_tokenService.BuildAccessToken(command));
			await Task.Delay(1000);
		}

		// Assert
		Assert.Equal(5, results.Count);
		foreach (var result in results)
		{
			Assert.NotNull(result);
			Assert.NotNull(result.Token);
			Assert.NotEmpty(result.Token);
			Assert.True(result.Expires > DateTime.UtcNow);
		}

		// All tokens should be different (due to different creation times)
		var uniqueTokens = results.Select(r => r.Token).Distinct().Count();
		Assert.Equal(5, uniqueTokens);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("user-with-special-chars!@#$%^&*()")]
	[InlineData("user-with-unicode-测试")]
	[InlineData("very-long-user-id-that-exceeds-normal-length-limits-and-should-still-work-correctly")]
	public void TokenService_WhenBuildAccessTokenCalledWithVariousUserIds_ShouldReturnValidTokens(string userId)
	{
		// Arrange
		var command = new BuildAccessTokenCommand { UserId = userId };

		// Act
		var result = _tokenService.BuildAccessToken(command);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Token);
		Assert.NotEmpty(result.Token);
		Assert.True(result.Expires > DateTime.UtcNow);

		// Verify the user ID is correctly embedded in the token
		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(result.Token);
		Assert.Equal(userId, token.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
	}

	[Fact]
	public void TokenService_WhenBuildAccessTokenCalled_ShouldReturnTokenWithCorrectIssuerAndAudience()
	{
		// Arrange
		var userId = "test-issuer-audience";
		var command = new BuildAccessTokenCommand { UserId = userId };

		// Act
		var result = _tokenService.BuildAccessToken(command);

		// Assert
		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(result.Token);

		// Verify issuer and audience are null as per the implementation
		Assert.Null(token.Issuer);
		Assert.Null(token.Audiences?.FirstOrDefault());
	}
}
