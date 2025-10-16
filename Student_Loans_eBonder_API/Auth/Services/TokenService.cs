using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Auth.Types.Responses;

namespace Student_Loans_eBonder_API.Auth.Services;

public partial class TokenService(ILogger<TokenService> logger, IConfiguration configuration)
{
	public BuildAccessTokenResponse BuildAccessToken(BuildAccessTokenCommand buildAccessTokenCommand)
	{
		ArgumentNullException.ThrowIfNull(buildAccessTokenCommand);

		LogBuildTokenAttemptMessage(logger, buildAccessTokenCommand.UserId);

		var claims = new List<Claim>()
		{
			new ("userId", buildAccessTokenCommand.UserId)
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTKey"]!));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var expires = DateTime.UtcNow.AddMonths(1);

		var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expires, signingCredentials: creds);

		var response = new BuildAccessTokenResponse
		{
			Token = new JwtSecurityTokenHandler().WriteToken(token),
			Expires = expires,
		};

		LogBuildTokenSuccessMessage(logger, buildAccessTokenCommand.UserId);

		return response;
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Attempt to build token for user {UserId}")]
	static partial void LogBuildTokenAttemptMessage(ILogger logger, string userId);

	[LoggerMessage(Level = LogLevel.Information, Message = "Successfully built token for user {UserId}")]
	static partial void LogBuildTokenSuccessMessage(ILogger logger, string userId);
}
