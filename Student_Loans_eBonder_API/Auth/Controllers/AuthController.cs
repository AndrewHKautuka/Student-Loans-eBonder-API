using System.Text.Json;

using Mapster;

using Microsoft.AspNetCore.Mvc;

using Student_Loans_eBonder_API.Auth.Services;
using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Auth.Types.Requests;
using Student_Loans_eBonder_API.Auth.Types.Responses;

namespace Student_Loans_eBonder_API.Auth.Controllers;
[Route("api/auth")]
[ApiController]
public partial class AuthController(ILogger<AuthController> logger, AuthService authService, TokenService tokenService) : ControllerBase
{
	[HttpPost("register-student")]
	public async Task<ActionResult<RegisterUserResponse>> RegisterStudent([FromBody] RegisterStudentRequest request)
	{
		LogRegisterStudentEndpointHitMessage(logger, JsonSerializer.Serialize(request));
		var (result, userId) = await authService.RegisterStudent(request).ConfigureAwait(true);

		if (result.Succeeded)
		{
			var tokenResponse = tokenService.BuildAccessToken(new BuildAccessTokenCommand { UserId = userId! }); // Ideally would use discrimated unions when added to C#
			var userResponse = tokenResponse.Adapt<RegisterUserResponse>();
			LogRegisterStudentEndpointSuccessfullResponseMessage(logger, JsonSerializer.Serialize(userResponse));
			return userResponse;
		}
		else
		{
			var errors = result.Errors;
			LogRegisterStudentEndpointFailedResponseMessage(logger, JsonSerializer.Serialize(result.Errors));
			return BadRequest(errors);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Register Student endpoint hit with request:\n{request}")]
	static partial void LogRegisterStudentEndpointHitMessage(ILogger logger, string request);

	[LoggerMessage(Level = LogLevel.Debug, Message = "Register Student endpoint successfull with response:\n{response}")]
	static partial void LogRegisterStudentEndpointSuccessfullResponseMessage(ILogger logger, string response);

	[LoggerMessage(Level = LogLevel.Debug, Message = "Register Student endpoint failed with response:\n{errors}")]
	static partial void LogRegisterStudentEndpointFailedResponseMessage(ILogger logger, string errors);
}
