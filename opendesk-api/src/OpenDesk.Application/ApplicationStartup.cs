using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Authorization;
using OpenDesk.Application.Identity;
using OpenDesk.Application.Interfaces;
using OpenDesk.Application.Persistence;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Authentication;
using System;

namespace OpenDesk.Application
{
	public static class ApplicationStartup
	{
		public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
		{
			// PERSISTENCE

			services.AddDbContext<OpenDeskDbContext>(o =>
			{
				var useInMemoryEnvVar = Environment.GetEnvironmentVariable("OPENDESK_USE_INMEMORY_DATABASE");
				if (useInMemoryEnvVar == "1" || useInMemoryEnvVar == "true")
				{
					o.UseInMemoryDatabase("OpenDeskDb");
				}
				else
				{
					o.UseNpgsql(configuration.GetConnectionString("DB"));
					o.UseSnakeCaseNamingConvention();
				}
			});

			services.AddScoped<IBlobSaver, LocalFileBlobSaver>();

			// IDENTITY

			services.AddIdentityCore<OpenDeskUser>() // "Core" version does not set up the cookies. We want to do it ourselves.
				.AddRoles<OpenDeskRole>()
				.AddEntityFrameworkStores<OpenDeskDbContext>()
				.AddClaimsPrincipalFactory<OpenDeskUserClaimsPrincipalFactory>(); // Custom factory for adding our own claims.

			// AUTHENTICATION & AUTHORIZATION

			services.AddScoped<IExternalAuthenticationVerifier, ExternalAuthenticationVerifier>();
			services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();

			services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
			services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

			services.AddAuthentication(o =>
			{
				o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
				.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
				{
					o.Cookie.Name = "OpenDeskAuth";
					o.Cookie.SameSite = isDevelopment ? SameSiteMode.None : SameSiteMode.Strict;

					o.Events.OnRedirectToAccessDenied = ctx =>
					{
						ctx.HttpContext.Response.StatusCode = 401;
						return Task.CompletedTask;
					};

					o.Events.OnRedirectToLogin = o.Events.OnRedirectToAccessDenied;
				});

			services.AddAuthorizationCore(o =>
			{
				// Replace the default policy so that cookies are used.
				o.DefaultPolicy = new AuthorizationPolicyBuilder()
					.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser()
					.Build();
			});

			return services;
		}
	}
}
