using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Common.Types.Models;
using Student_Loans_eBonder_API.Profile.Types.Models;

namespace Student_Loans_eBonder_API;

public class ApplicationDBContext([NotNull] DbContextOptions options) : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options)
{
	protected override void OnModelCreating(ModelBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		builder.Model.GetEntityTypes()
			.Where(e => typeof(ITimestampEntity).IsAssignableFrom(e.ClrType))
			.Select(entityType => entityType.ClrType)
			.ToList()
			.ForEach(clrType =>
			{
				builder.Entity(clrType)
					.Property(nameof(ITimestampEntity.CreatedAt))
					.HasDefaultValueSql("CURRENT_TIMESTAMP")
					.ValueGeneratedOnAdd();

				builder.Entity(clrType)
					.Property(nameof(ITimestampEntity.UpdatedAt))
					.HasDefaultValueSql("CURRENT_TIMESTAMP")
					.ValueGeneratedOnAddOrUpdate();
			});

		base.OnModelCreating(builder);

		// MUST explicitly specify table names to remove AspNet prefix
		builder.Entity<User>().ToTable("user");
		builder.Entity<Role>().ToTable("role");
		builder.Entity<UserRole>().ToTable("user_role");
		builder.Entity<UserClaim>().ToTable("user_claim");
		builder.Entity<UserLogin>().ToTable("user_login");
		builder.Entity<RoleClaim>().ToTable("role_claim");
		builder.Entity<UserToken>().ToTable("user_token");
	}

	public DbSet<Name> Name { get; set; }
	public DbSet<NameComponent> NameComponent { get; set; }
	public DbSet<Student.Types.Models.Student> Student { get; set; }
	public DbSet<UserProfile> UserProfile { get; set; }
}
