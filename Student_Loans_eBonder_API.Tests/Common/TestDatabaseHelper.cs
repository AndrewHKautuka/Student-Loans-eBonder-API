using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Student_Loans_eBonder_API.Tests.Common;

/// <summary>
/// Generic helper class for creating and managing test databases using Entity Framework InMemory provider.
/// This class provides reusable functionality for setting up test databases across different test modules.
/// </summary>
public static class TestDatabaseHelper
{
	/// <summary>
	/// Creates a new ApplicationDBContext configured with InMemory database for testing.
	/// </summary>
	/// <param name="databaseName">Optional database name. If not provided, a unique name will be generated.</param>
	/// <returns>A configured ApplicationDBContext instance.</returns>
	public static ApplicationDBContext CreateTestDbContext(string? databaseName = null)
	{
		var options = CreateTestDbContextOptions(databaseName);
		return new ApplicationDBContext(options);
	}

	/// <summary>
	/// Creates DbContextOptions configured with InMemory database for testing.
	/// </summary>
	/// <param name="databaseName">Optional database name. If not provided, a unique name will be generated.</param>
	/// <returns>DbContextOptions configured for testing.</returns>
	public static DbContextOptions<ApplicationDBContext> CreateTestDbContextOptions(string? databaseName = null)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDBContext>();
		optionsBuilder.UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString());
		return optionsBuilder.Options;
	}

	/// <summary>
	/// Creates a service collection with test database configuration.
	/// Useful for integration tests that need to configure services with a test database.
	/// </summary>
	/// <param name="databaseName">Optional database name. If not provided, a unique name will be generated.</param>
	/// <returns>ServiceCollection configured with test database.</returns>
	public static IServiceCollection CreateTestServiceCollection(string? databaseName = null)
	{
		var services = new ServiceCollection();

		// Add Entity Framework with InMemory database
		services.AddDbContext<ApplicationDBContext>(options =>
			options.UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString()));

		// Add logging (useful for testing services that require ILogger)
		services.AddLogging(builder => builder.AddConsole());

		return services;
	}

	/// <summary>
	/// Ensures the database is created and ready for use.
	/// </summary>
	/// <param name="context">The database context to initialize.</param>
	public static async Task EnsureDatabaseCreatedAsync(ApplicationDBContext context)
	{
		ArgumentNullException.ThrowIfNull(context);
		await context.Database.EnsureCreatedAsync().ConfigureAwait(true);
	}

	/// <summary>
	/// Cleans up the test database by removing all data.
	/// </summary>
	/// <param name="context">The database context to clean up.</param>
	public static async Task CleanupDatabaseAsync(ApplicationDBContext context)
	{
		ArgumentNullException.ThrowIfNull(context);
		// Remove all entities from all tables
		context.Users.RemoveRange(context.Users);
		context.Roles.RemoveRange(context.Roles);
		context.UserRoles.RemoveRange(context.UserRoles);
		context.UserClaims.RemoveRange(context.UserClaims);
		context.UserLogins.RemoveRange(context.UserLogins);
		context.RoleClaims.RemoveRange(context.RoleClaims);
		context.UserTokens.RemoveRange(context.UserTokens);
		context.Names.RemoveRange(context.Names);
		context.NameComponents.RemoveRange(context.NameComponents);
		context.Students.RemoveRange(context.Students);
		context.UserProfiles.RemoveRange(context.UserProfiles);

		await context.SaveChangesAsync().ConfigureAwait(true);
	}

	/// <summary>
	/// Disposes the context and cleans up resources.
	/// </summary>
	/// <param name="context">The database context to dispose.</param>
	public static void DisposeContext(ApplicationDBContext context)
	{
		ArgumentNullException.ThrowIfNull(context);
		context.Dispose();
	}
}
