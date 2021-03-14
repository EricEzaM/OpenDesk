using MediatR;
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
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenDesk.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenDesk.API
{
	public class Startup
	{
		private readonly IWebHostEnvironment _env;

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
			_env = env;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddInfrastructure(Configuration);

			services.AddMediatR(typeof(Startup));

			services.AddHttpClient();

			if (_env.IsDevelopment())
			{
				services.AddCors(o =>
				{
					o.AddDefaultPolicy(cb =>
					{
						cb.AllowAnyOrigin();
						cb.AllowAnyMethod();
						cb.AllowAnyHeader();
					});
				});
			}

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

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "office-images")),
				RequestPath = "/office-images"
			});

			app.UseRouting();

			if (env.IsDevelopment())
			{
				app.UseCors();
			}

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				//endpoints.MapControllers();
				endpoints.MapControllers();
			});
		}
	}
}
