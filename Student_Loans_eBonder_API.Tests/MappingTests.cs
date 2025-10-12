using Mapster;

using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Auth.Types.Requests;
using Student_Loans_eBonder_API.Auth.Types.Responses;
using Student_Loans_eBonder_API.Common;

namespace Student_Loans_eBonder_API.Tests;

public class MappingTests() : IAsyncLifetime
{
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
	public void MapsterMapping_WhenMappingRegisterStudentCommandToRegisterUserCommand_ShouldCreateValidRegisterUserCommand()
	{
		var source = new RegisterStudentRequest { Email = "test@example.com", Password = "1234!@#$" };
		var dest = source.Adapt<RegisterUserCommand>();

		Assert.Equal("test@example.com", dest.Email);
		Assert.Equal("1234!@#$", dest.Password);
	}

	[Fact]
	public void MapsterMapping_WhenMappingRegisterUserCommandToUser_ShouldCreateValidUserAndPassword()
	{
		var source = new RegisterUserCommand { Email = "test@example.com", Password = "1234!@#$" };
		var dest = source.Adapt<(User User, string Password)>();

		Assert.Equal("test@example.com", dest.User.Email);
		Assert.Equal("test@example.com", dest.User.UserName);
		Assert.Equal("1234!@#$", dest.Password);
	}

	[Fact]
	public void MapsterMapping_WhenMappingBuildTokenResponseToRegisterUserResponse_ShouldCreateValidRegisterUserResponse()
	{
		var token = "ThisIsAnExampleTokenForTestingPurposesOnly123456789";
		var expiryDate = new DateTime(2030, 1, 1, 12, 30, 15, DateTimeKind.Utc);

		var source = new BuildAccessTokenResponse { Token = token, Expires = expiryDate };
		var dest = source.Adapt<RegisterUserResponse>();

		Assert.Equal(token, dest.Token);
		Assert.Equal(expiryDate, dest.Expires);
	}
}
