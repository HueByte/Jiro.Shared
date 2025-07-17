using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


/// <summary>
/// Provides extension methods for registering Entity Framework Core DbContext types with various database providers.
/// </summary>
namespace Jiro.Shared.Extensions;

public static class ServiceExtensions
{
	/// <summary>
	/// Registers a MySQL DbContext of type <typeparamref name="T"/> with the specified connection string.
	/// </summary>
	/// <typeparam name="T">The type of the DbContext to register.</typeparam>
	/// <param name="services">The service collection to add the DbContext to.</param>
	/// <param name="connectionString">The MySQL connection string.</param>
	/// <returns>The updated <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddMySqlContext<T>(this IServiceCollection services, string connectionString) where T : DbContext
	{
		services.AddDbContext<T>(options =>
		{
			options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
				x => x.MigrationsAssembly(typeof(T).Assembly.GetName().Name));
		});

		return services;
	}

	/// <summary>
	/// Registers a SQLite DbContext of type <typeparamref name="T"/> with the specified connection string.
	/// </summary>
	/// <typeparam name="T">The type of the DbContext to register.</typeparam>
	/// <param name="services">The service collection to add the DbContext to.</param>
	/// <param name="connectionString">The SQLite connection string or file path.</param>
	/// <returns>The updated <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddSqliteContext<T>(this IServiceCollection services, string connectionString) where T : DbContext
	{
		services.AddDbContext<T>(options =>
		{
			options.UseSqlite($"Data Source={connectionString}",
				x => x.MigrationsAssembly(typeof(T).Assembly.GetName().Name));
		});

		return services;
	}
}
