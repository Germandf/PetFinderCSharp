using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetFinder.Areas.Identity;
using PetFinder.Data;
using PetFinder.Models;
using PetFinderApi.Data.Interfaces;
using PetFinderApi.Data.Services;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PetFinderApi
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
            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "PetFinderApi",
                    Version = "v1",
                    Description = "PetFinderApi's official documentation. " +
                    "Created with educational purposes for Eternet SRL by Huertas José and Germán De Francesco.",
                    TermsOfService = new Uri("https://petfinder.com/api/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Eternet SRL",
                        Email = "develop@eternet.cc",
                        Url = new Uri("https://www.eternet.cc"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://petfinder.com/api/license"),
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

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

            // ===== Add Identity ========
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<PetFinderContext>()
                .AddDefaultTokenProviders();

            // ===== Add Jwt Authentication ========
            Configuration["JwtKey"] = Environment.GetEnvironmentVariable("PetFinderJWTSecret"); // Random password secret
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                           Configuration["JwtKey"]
                        )),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<PetService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetFinderApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
