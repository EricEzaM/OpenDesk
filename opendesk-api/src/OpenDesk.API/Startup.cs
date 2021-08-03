using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenDesk.API.Errors;
using OpenDesk.Application;
using OpenDesk.Application.Authentication;
using OpenDesk.Application.Behaviours;
using OpenDesk.Application.Common;
using OpenDesk.Application.Exceptions;
using System;
using System.IO;
using System.Linq;

namespace OpenDesk.API
{
	public class Startup
	{
		private readonly IWebHostEnvironment _env;

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
			_env = env;
			CreateBlobsFolder(env);
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			bool isDevelopment = _env.IsDevelopment();

			services.Configure<ApplicationOptions>(Configuration.GetSection(ApplicationOptions.SECTION_NAME));
			services.Configure<AuthenticationOptions>(Configuration.GetSection(AuthenticationOptions.SECTION_NAME));

			services.AddProblemDetails(o =>
			{
				o.ExceptionDetailsPropertyName = "internalErrors";

				o.Map<EntityNotFoundException>(ex => new ProblemDetails()
				{
					Title = $"{ex.EntityName} could not be found.",
					Type = "https://example.net/entity-not-found",
					Status = StatusCodes.Status404NotFound,
				});

				o.Map<ValidationException>(ex => 
					new FluentValidationProblemDetails(ex.Errors.Select(f => new FluentValidationProblemDetailsError(f)))
					{
						Title = ex.Message,
						Type = "https://example.net/validation",
						Status = StatusCodes.Status400BadRequest
					});

				o.MapToStatusCode<NotSupportedException>(StatusCodes.Status501NotImplemented);
				o.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
			});

			services.ConfigureApplicationServices(isDevelopment);

			services.AddMediatR(typeof(Startup), typeof(ApplicationStartup));
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(StringTrimmingBehaviour<,>));
			services.AddValidatorsFromAssembly(typeof(Startup).Assembly);

			services.AddHttpClient();

			if (_env.IsDevelopment())
			{
				services.AddCors(o =>
				{
					o.AddDefaultPolicy(cb =>
					{
						cb.WithOrigins("http://localhost:3000");
						cb.AllowAnyMethod();
						cb.AllowAnyHeader();
						cb.AllowCredentials();
						cb.WithExposedHeaders("Content-Length");
					});
				});
			}

			services.AddControllers()
				.AddApplicationPart(typeof(ApplicationStartup).Assembly)
				.AddProblemDetailsConventions(); // Adds MVC conventions to work better with the ProblemDetails middleware.
		

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

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			app.UseProblemDetails();

			app.UseHttpsRedirection();

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "static", "blobs")),
				RequestPath = "/static/blobs"
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
				endpoints.MapControllers();
			});
		}

		private void CreateBlobsFolder(IWebHostEnvironment env)
		{
			var path = Path.Combine(env.ContentRootPath, "static", "blobs");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
	}
}
