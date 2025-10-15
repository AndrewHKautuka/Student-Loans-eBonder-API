using Microsoft.AspNetCore.Identity;

namespace Student_Loans_eBonder_API.Common;

public static class IdentityConfig
{
	public static void SetIdentityOptions(IdentityOptions options)
	{
		ArgumentNullException.ThrowIfNull(options);
		options.User.RequireUniqueEmail = true;
	}
}
