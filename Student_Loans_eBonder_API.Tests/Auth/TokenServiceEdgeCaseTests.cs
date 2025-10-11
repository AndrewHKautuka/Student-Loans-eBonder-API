using System.IdentityModel.Tokens.Jwt;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using Student_Loans_eBonder_API.Auth.Services;
using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Common;

namespace Student_Loans_eBonder_API.Tests.Auth;

public class TokenServiceEdgeCaseTests : IAsyncLifetime
{
	private readonly Mock<ILogger<TokenService>> _mockLogger;
	private readonly Mock<IConfiguration> _mockConfiguration;

	public TokenServiceEdgeCaseTests()
	{
		_mockLogger = new Mock<ILogger<TokenService>>();
		_mockConfiguration = new Mock<IConfiguration>();
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
	public void TokenService_WhenJWTKeyIsNull_ShouldThrowException()
	{
		// Arrange
		_mockConfiguration.Setup(x => x["JWTKey"]).Returns((string?)null);
		var tokenService = new TokenService(_mockLogger.Object, _mockConfiguration.Object);
		var command = new BuildAccessTokenCommand { UserId = "test-user" };

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => tokenService.BuildAccessToken(command));
	}

	[Fact]
	public void TokenService_WhenJWTKeyIsEmpty_ShouldThrowException()
	{
		// Arrange
		_mockConfiguration.Setup(x => x["JWTKey"]).Returns("");
		var tokenService = new TokenService(_mockLogger.Object, _mockConfiguration.Object);
		var command = new BuildAccessTokenCommand { UserId = "test-user" };

		// Act & Assert
		Assert.Throws<ArgumentException>(() => tokenService.BuildAccessToken(command));
	}

	[Fact]
	public void TokenService_WhenJWTKeyIsTooShort_ShouldThrowException()
	{
		// Arrange
		_mockConfiguration.Setup(x => x["JWTKey"]).Returns("short");
		var tokenService = new TokenService(_mockLogger.Object, _mockConfiguration.Object);
		var command = new BuildAccessTokenCommand { UserId = "test-user" };

		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => tokenService.BuildAccessToken(command));
	}

	[Fact]
	public void TokenService_WhenUserIdIsNull_ShouldThrowException()
	{
		// Arrange
		_mockConfiguration.Setup(x => x["JWTKey"])
			.Returns("ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456789");
		var tokenService = new TokenService(_mockLogger.Object, _mockConfiguration.Object);
		var command = new BuildAccessTokenCommand { UserId = null! };

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => tokenService.BuildAccessToken(command));
	}

	[Theory]
	[InlineData("key-with-special-chars!@#$%^&*()")]
	[InlineData("key-with-unicode-测试😘🙌◐ↁ⩓ᾅϟ")]
	[InlineData("key-with-newlines\n,-backspaces\band-tabs\t")]
	[InlineData("key-with-very-long-content-that-exceeds-normal-length-limits-and-should-still-work-correctly-because-jwt-keys-can-be-very-long")]
	public void TokenService_WhenJWTKeyHasSpecialCharacters_ShouldWorkCorrectly(string jwtKey)
	{
		// Arrange
		_mockConfiguration.Setup(x => x["JWTKey"]).Returns(jwtKey);
		var tokenService = new TokenService(_mockLogger.Object, _mockConfiguration.Object);
		var command = new BuildAccessTokenCommand { UserId = "test-user" };

		// Act
		var result = tokenService.BuildAccessToken(command);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Token);
		Assert.NotEmpty(result.Token);
		Assert.True(result.Expires > DateTime.UtcNow);

		// Verify the token can be parsed
		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(result.Token);
		Assert.Equal("test-user", token.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
	}

	[Fact]
	public async Task TokenService_WhenCalledConcurrently_ShouldReturnValidTokens()
	{
		// Arrange
		_mockConfiguration.Setup(x => x["JWTKey"])
			.Returns("ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456789");
		var tokenService = new TokenService(_mockLogger.Object, _mockConfiguration.Object);

		var tasks = new List<Task<Student_Loans_eBonder_API.Auth.Types.Responses.BuildAccessTokenResponse>>();
		var userIds = Enumerable.Range(1, 10).Select(i => $"concurrent-user-{i}").ToList();

		// Act
		foreach (var userId in userIds)
		{
			tasks.Add(Task.Run(() =>
			{
				var command = new BuildAccessTokenCommand { UserId = userId };
				return tokenService.BuildAccessToken(command);
			}));
		}

		var results = await Task.WhenAll(tasks);

		// Assert
		Assert.Equal(10, results.Length);
		foreach (var result in results)
		{
			Assert.NotNull(result);
			Assert.NotNull(result.Token);
			Assert.NotEmpty(result.Token);
			Assert.True(result.Expires > DateTime.UtcNow);
		}

		// All tokens should be unique
		var tokens = results.Select(r => r.Token).ToList();
		var uniqueTokens = tokens.Distinct().Count();
		Assert.Equal(10, uniqueTokens);
	}

	[Fact]
	public void TokenService_WhenTokenIsGenerated_ShouldBeVerifiableWithSameKey()
	{
		// Arrange
		var jwtKey = "ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456789";
		_mockConfiguration.Setup(x => x["JWTKey"]).Returns(jwtKey);
		var tokenService = new TokenService(_mockLogger.Object, _mockConfiguration.Object);
		var command = new BuildAccessTokenCommand { UserId = "verification-test" };

		// Act
		var result = tokenService.BuildAccessToken(command);

		// Assert
		var handler = new JwtSecurityTokenHandler();
#pragma warning disable CA5404 // Do not disable token validation checks
		var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
				System.Text.Encoding.UTF8.GetBytes(jwtKey)),
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};
#pragma warning restore CA5404 // Do not disable token validation checks

		// This should not throw an exception if the token is valid
		var principal = handler.ValidateToken(result.Token, validationParameters, out var validatedToken);
		Assert.NotNull(principal);
		Assert.NotNull(validatedToken);
	}

	[Fact]
	public void TokenService_WhenTokenIsGenerated_ShouldContainCorrectClaimTypes()
	{
		// Arrange
		_mockConfiguration.Setup(x => x["JWTKey"])
			.Returns("ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456789");
		var tokenService = new TokenService(_mockLogger.Object, _mockConfiguration.Object);
		var command = new BuildAccessTokenCommand { UserId = "claim-test" };

		// Act
		var result = tokenService.BuildAccessToken(command);

		// Assert
		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(result.Token);

		// Verify claim structure
		var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "userId");
		Assert.NotNull(userIdClaim);
		Assert.Equal("claim-test", userIdClaim.Value);
		Assert.Equal("userId", userIdClaim.Type);
	}

	[Fact]
	public void TokenService_WhenTokenIsGenerated_ShouldHaveCorrectTokenStructure()
	{
		// Arrange
		_mockConfiguration.Setup(x => x["JWTKey"])
			.Returns("ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456789");
		var tokenService = new TokenService(_mockLogger.Object, _mockConfiguration.Object);
		var command = new BuildAccessTokenCommand { UserId = "structure-test" };

		// Act
		var result = tokenService.BuildAccessToken(command);

		// Assert
		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(result.Token);

		// Verify token structure
		Assert.NotNull(token.Header);
		Assert.NotNull(token.Payload);
		Assert.Equal("HS256", token.Header.Alg);
		Assert.Equal("JWT", token.Header.Typ);
		Assert.Null(token.Issuer);
		Assert.Null(token.Audiences?.FirstOrDefault());
		Assert.True(token.ValidFrom <= DateTime.UtcNow);
		Assert.True(token.ValidTo > DateTime.UtcNow);
		Assert.True(token.ValidTo <= DateTime.UtcNow.AddMonths(1).AddMinutes(1));
	}
}
