using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetFinder.Data;
using PetFinder.Models;
using System;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Components.Server;
using PetFinder.Areas.Identity;
using PetFinder.Areas.Identity.Helper;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Serilog.Ui.Web;
using Serilog.Ui.MsSqlServerProvider;

namespace PetFinder
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var mvcBuilder = services.AddControllersWithViews();
            services.AddSerilogUi(mvcBuilder, options => options
                .EnableAuthorization(authOptions =>
                {
                    authOptions.Roles = new[] { ApplicationUserService.ROLE_ADMIN };
                })
                .UseSqlServer(Environment.GetEnvironmentVariable("SQLServerPetfinder"), "Logs")
            );

            Configuration["UrlApiController"] = Configuration.GetValue<string>("apiCommentsURL");

            services.AddDbContext<PetFinderContext>(options =>
                    options.UseSqlServer(Environment.GetEnvironmentVariable("SQLServerPetfinder")),
                    ServiceLifetime.Transient
                  );

            services.AddSingleton(
                (ILogger)new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.MSSqlServer(
                        connectionString: Environment.GetEnvironmentVariable("SQLServerPetfinder"),
                        sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs" })
                    .CreateLogger()
            );
            services.AddDefaultIdentity<ApplicationUser>(options => 
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;

            }).AddRoles<IdentityRole>().
            AddEntityFrameworkStores<PetFinderContext>().
            AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>().
            AddErrorDescriber<AppErrorDescriber>();

            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            services.AddScoped<SignInManager<ApplicationUser>, SignInManager<ApplicationUser>>();

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IAnimalTypeService, AnimalTypeService>();
            services.AddScoped<IGenderService, GenderService>();
            services.AddScoped<IPetService, PetService>();

            services.AddScoped<IApplicationUserService, ApplicationUserService>();

            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAuthJwtService, AuthJwtService>();

            services.AddHttpContextAccessor();

            services.AddControllers();  
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
