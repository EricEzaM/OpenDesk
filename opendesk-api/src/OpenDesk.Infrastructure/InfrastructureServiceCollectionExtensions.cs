using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenDesk.Application.Common.Interfaces;
using OpenDesk.Infrastructure.Identity;
using OpenDesk.Infrastructure.Persistence;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Infrastructure.Authorization;

namespace OpenDesk.Infrastructure
{
	public static class InfrastructureServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
		{
			// PERSISTENCE
			
			services.AddDbContext<OpenDeskDbContext>(o =>
			{
				o.UseInMemoryDatabase("OpenDeskDb");
			});

			services.AddScoped<IOpenDeskDbContext>(provider => provider.GetService<OpenDeskDbContext>());
			services.AddScoped<IBlobSaver, LocalFileBlobSaver>();

			// IDENTITY

			services.AddIdentityCore<OpenDeskUser>() // "Core" version does not set up the cookies. We want to do it ourselves.
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<OpenDeskDbContext>()
				.AddClaimsPrincipalFactory<OpenDeskUserClaimsPrincipalFactory>(); // Custom factory for adding our own claims.

			// AUTHENTICATION & AUTHORIZATION

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

			// APPLICATION SERVICES

			services.AddTransient<IIdentityService, IdentityService>();

			return services;
		}
	}
}
