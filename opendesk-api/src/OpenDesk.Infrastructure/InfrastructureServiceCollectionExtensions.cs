using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenDesk.Application.Common.Interfaces;
using OpenDesk.Infrastructure.Identity;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure
{
	public static class InfrastructureServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			// PERSISTENCE
			
			services.AddDbContext<OpenDeskDbContext>(o =>
			{
				o.UseInMemoryDatabase("OpenDeskDb");
			});

			services.AddScoped<IOpenDeskDbContext>(provider => provider.GetService<OpenDeskDbContext>());

			// IDENTITY

			services.AddIdentityCore<OpenDeskUser>() // "Core" version does not set up the cookies. We want to do it ourselves.
				.AddEntityFrameworkStores<OpenDeskDbContext>();

			// AUTHENTICATION & AUTHORIZATION

			services.AddAuthentication(o =>
			{
				o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
				.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
				{
					o.Cookie.Name = "OpenDeskUser";
					o.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;

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
