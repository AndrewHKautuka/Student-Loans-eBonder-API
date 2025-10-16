using Mapster;

using Student_Loans_eBonder_API.Auth.Types.Commands;
using Student_Loans_eBonder_API.Auth.Types.Models;

namespace Student_Loans_eBonder_API.Common;

public static class MapsterConfig
{
	private static bool _hasBeenSetup;
	private static readonly Lock _setupLock = new();

	public static void SetupMapsterAdapterConfig()
	{
		if (_hasBeenSetup)
		{
			return;
		}

		lock (_setupLock)
		{
			if (_hasBeenSetup)
			{
				return;
			}

			TypeAdapterConfig<RegisterUserCommand, (User User, string Password)>.NewConfig()
				.Map(dest => dest.User, src => src)
				.Map(dest => dest.User.UserName, src => src.Email)
				.Map(dest => dest.Password, src => src.Password);

			_hasBeenSetup = true;
		}
	}
}
