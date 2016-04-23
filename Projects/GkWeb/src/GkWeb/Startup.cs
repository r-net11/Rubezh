using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.WebSockets.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GkWeb
{
	public class Startup
	{
		public Startup(IHostingEnvironment env) {
			// Set up configuration sources.
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			// Add framework services.
			services.AddMvc();
			services.AddSignalR();
			//Other middleware
			services.AddAuthentication(options =>
			{
				options.SignInScheme = "MyAuthenticationScheme";
			});

			services.AddAuthorization();
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
			app.UseWebSockets();

			app.UseDefaultFiles();
			app.UseStaticFiles();

			//Other configurations.
			app.UseCookieAuthentication(options =>
			{
				options.AuthenticationScheme = "Automatic";
				options.LoginPath = new PathString("/api/Auth/Login");
				options.AccessDeniedPath = new PathString("/signin/");
				options.AutomaticAuthenticate = true;
			});

			app.UseSignalR();

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "api",
					template: "api/{controller}/");
			});
		}

		// Entry point for the application.
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
