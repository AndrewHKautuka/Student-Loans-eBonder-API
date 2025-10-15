using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Common;

namespace Student_Loans_eBonder_API.Tests.Common;

/// <summary>
/// Base class for tests that require database access.
/// Provides common setup and teardown functionality for database-dependent tests.
/// </summary>
public abstract class DatabaseTestBase : IAsyncLifetime
{
	private ApplicationDBContext _testDBContext = null!;
	protected ApplicationDBContext TestDbContext
	{
		get => _testDBContext;
		private set
		{
			_testDBContext = value;
			UserStore = new(_testDBContext);
		}
	}
	protected static Mock<ILogger<T>> MockLogger<T>() where T : class => new Mock<ILogger<T>>();
	private UserStore<User, Role, ApplicationDBContext, string> _userStore = null!;
	protected UserStore<User, Role, ApplicationDBContext, string> UserStore
	{
		get => _userStore;
		private set
		{
			_userStore = value;
			UserManager = CreateUserManager(UserStore);
		}
	}
	protected UserManager<User> UserManager { get; private set; } = null!;

	/// <summary>
	/// Gets a unique database name for this test instance.
	/// </summary>
	protected virtual string DatabaseName => $"{GetType().Name}_{Guid.NewGuid():N}";

	/// <summary>
	/// Called before each test method. Sets up the test database.
	/// </summary>
	public virtual async Task InitializeAsync()
	{
		// Setup Mapster configuration
		MapsterConfig.SetupMapsterAdapterConfig();

		// Create test database context
		TestDbContext = TestDatabaseHelper.CreateTestDbContext(DatabaseName);
		await TestDatabaseHelper.EnsureDatabaseCreatedAsync(TestDbContext).ConfigureAwait(true);
	}

	/// <summary>
	/// Called after each test method. Cleans up the test database.
	/// </summary>
	public virtual async Task DisposeAsync()
	{
		if (TestDbContext != null)
		{
			await TestDatabaseHelper.CleanupDatabaseAsync(TestDbContext).ConfigureAwait(true);
			TestDatabaseHelper.DisposeContext(TestDbContext);
		}
	}

	/// <summary>
	/// Helper method to save changes to the test database.
	/// </summary>
	protected async Task SaveChangesAsync()
	{
		await TestDbContext.SaveChangesAsync().ConfigureAwait(true);
	}

	private static UserManager<User> CreateUserManager(IUserStore<User> store)
	{
		var identityOptions = new IdentityOptions();
		IdentityConfig.SetIdentityOptions(identityOptions);

		var options = Options.Create(identityOptions);
		var passwordHasher = new PasswordHasher<User>();
		var userValidators = new List<IUserValidator<User>> { new UserValidator<User>() };
		var passwordValidators = new List<IPasswordValidator<User>> { new PasswordValidator<User>() };
		var normalizer = new UpperInvariantLookupNormalizer();
		var errorDescriber = new IdentityErrorDescriber();
		var services = new ServiceCollection()
			.AddLogging(builder => builder.AddDebug())
			.BuildServiceProvider();
		var logger = services.GetRequiredService<ILogger<UserManager<User>>>();

		return new UserManager<User>(
			store,
			options,
			passwordHasher,
			userValidators,
			passwordValidators,
			normalizer,
			errorDescriber,
			services,
			logger);
	}
}
