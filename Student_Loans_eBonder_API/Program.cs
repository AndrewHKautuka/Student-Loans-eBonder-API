
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Common.Extensions;

namespace Student_Loans_eBonder_API;

internal static class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		var configuration = builder.Configuration;

		// Add services to the container.

		builder.Services.AddControllers();
		builder.Services.AddAuthorization();
		// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
		builder.Services.AddOpenApi();

		builder.Services.AddIdentity<User, Role>(options =>
		{
			options.User.RequireUniqueEmail = true;
		}).AddRoles<Role>().AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();
		builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("APIDatabase"), options => options.MapDatabaseEnums()).UseSnakeCaseNamingConvention());

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.MapOpenApi();
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();


		app.MapControllers();

		app.Run();
	}
}
