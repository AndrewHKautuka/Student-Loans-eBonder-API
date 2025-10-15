using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

using Student_Loans_eBonder_API.Profile.Types.Models;
using Student_Loans_eBonder_API.Student.Types.Models;

namespace Student_Loans_eBonder_API.Common.Extensions;

public static class NpgsqlDbContextOptionsBuilderExtensions
{
	public static NpgsqlDbContextOptionsBuilder MapDatabaseEnums(this NpgsqlDbContextOptionsBuilder options)
	{
		ArgumentNullException.ThrowIfNull(options);
		return options.MapEnum<NameComponentType>()
					  .MapEnum<Sex>();
	}
}
