using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenDesk.Application.Common;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API
{
	public class Program
	{
		public async static Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;

				var options = services.GetRequiredService<IOptions<ApplicationOptions>>();

				try
				{
					if (options.Value.IsDemo)
					{
						await OpenDeskDbContextSeed.SeedDemo(services);
					}
				}
				catch (Exception ex)
				{
					var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred while migrating or seeding the database.");

					throw;
				}
			}

			await host.RunAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
