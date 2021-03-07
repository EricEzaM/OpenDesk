using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<IdentityDbContext>(o =>
			{
				o.UseInMemoryDatabase("Test");
			});

			services.AddIdentityCore<IdentityUser>() // "Core" version does not set up the cookies. We want to do it ourselves.
				.AddRoles<IdentityRole>()
				.AddSignInManager()
				.AddEntityFrameworkStores<IdentityDbContext>();

			services.AddAuthentication(o =>
			{
				o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
				.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
				{
					o.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;

					o.Events.OnRedirectToAccessDenied = ctx =>
					{
						ctx.HttpContext.Response.StatusCode = 401;
						return Task.CompletedTask;
					};

					o.Events.OnRedirectToLogin = o.Events.OnRedirectToAccessDenied;
				});

			services.AddAuthorization(o =>
			{
				// Replace the default policy so that cookies are used.
				o.DefaultPolicy = new AuthorizationPolicyBuilder()
					.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser()
					.Build();
			});
			
			services.AddHttpClient();

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenDesk", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenDesk v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
