
using Mapster;

using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Auth.Types.Requests;
using Student_Loans_eBonder_API.Common;

using Xunit.Abstractions;

namespace Student_Loans_eBonder_API.Tests;

public class MappingTests(ITestOutputHelper output) : IAsyncLifetime
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
}
