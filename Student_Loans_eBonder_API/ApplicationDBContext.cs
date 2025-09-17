using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

namespace Student_Loans_eBonder_API;

internal class ApplicationDBContext : DbContext
{
	public ApplicationDBContext([NotNull] DbContextOptions options) : base(options)
	{
	}
}
