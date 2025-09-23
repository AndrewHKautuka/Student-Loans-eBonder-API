using Mapster;

using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Auth.Types.Requests;

namespace Student_Loans_eBonder_API.Common;

internal static class MapsterConfig
{
	public static void SetupMapsterAdapterConfig()
	{
		TypeAdapterConfig<RegisterUserCommand, (User User, string Password)>.NewConfig()
			.Map(dest => dest.User, src => src)
			.Map(dest => dest.User.UserName, src => src.Email)
			.Map(dest => dest.Password, src => src.Password);
	}
}
