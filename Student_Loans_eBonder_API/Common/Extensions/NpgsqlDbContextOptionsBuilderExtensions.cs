using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

using Student_Loans_eBonder_API.Profile.Types.Models;
using Student_Loans_eBonder_API.Student.Types.Models;

namespace Student_Loans_eBonder_API.Common.Extensions;

internal static class NpgsqlDbContextOptionsBuilderExtensions
{
	public static NpgsqlDbContextOptionsBuilder MapDatabaseEnums(this NpgsqlDbContextOptionsBuilder options)
	{
		return options.MapEnum<NameComponentType>()
					  .MapEnum<Sex>();
	}
}
