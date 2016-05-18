#region Usings

using GkWeb.Hubs;
using GkWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Cors.Internal;

#endregion

namespace GkWeb
{
	[Authorize]
	public class Startup
	{
		public Startup(IHostingEnvironment env) {
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment()) {
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			// Add framework services.
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAllOrigins", builder => {
					builder.AllowAnyOrigin();
				});
			});

			services.Configure<MvcOptions>(options =>
			{
				options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigins"));
			});

			services.AddMvc();
			//Other middleware
			services.AddAuthentication();
			services.AddAuthorization();

			services.AddSignalR(options => {
				options.Hubs.EnableDetailedErrors = true;				
				options.Transports.DisconnectTimeout = new System.TimeSpan(0, 0, 15);
				options.Transports.KeepAlive = new System.TimeSpan(0, 0, 5);
			});

			services.AddSingleton(new Bootstrapper());
			services.AddSingleton<ClientManager>();
			services.AddSingleton(RubezhClient.ClientManager.FiresecService);
			services.AddSingleton<GkHubProxy>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment()) {
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else {
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseCors("AllowSpecificOrigin");

			app.UseWebSockets();

			app.UseStaticFiles();

			//Other configurations.
			var options = new CookieAuthenticationOptions();
			options.AuthenticationScheme = "Automatic";
			options.LoginPath = new PathString("/Logon/Login");
			options.AccessDeniedPath = new PathString("/signin/");
			options.AutomaticAuthenticate = true;
			app.UseCookieAuthentication(options);

			app.UseSignalR();

			app.UseMvc(
				routes => {					
					routes.MapRoute(
						name: "default",
						template: "{controller=Home}/{action=Index}/{id?}");
				});
		}
	}
}
