using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ethos.Application;
using Ethos.Application.Identity;
using Ethos.Application.Seed;
using Ethos.Domain.Entities;
using Ethos.EntityFrameworkCore;
using Ethos.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Ethos.Web.Host
{
    /// <summary>
    /// The startup class.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes the startup with the configuration.
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtConfig>(_configuration.GetSection(nameof(JwtConfig)));
            services.Configure<EmailConfig>(_configuration.GetSection(nameof(EmailConfig)));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("Default"));
            });

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "JwtBearer";
                    options.DefaultChallengeScheme = "JwtBearer";
                })
                .AddJwtBearer("JwtBearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Secret"])),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = _configuration["JwtConfig:TokenIssuer"],
                        ValidAudience = _configuration["JwtConfig:ValidAudience"],
                        ValidateLifetime = true,
                    };
                });

            // add application module
            services.AddApplicationModule();
            services.AddRepositories();
            services.AddQueries();

            services.AddHttpContextAccessor();

            services.AddControllers()
                .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(IEthosWebAssemblyMarker).Assembly));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ethos",
                    Version = "v1",
                });
                var path = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory));
                foreach (var filePath in Directory.GetFiles(path, "*.xml"))
                {
                    c.IncludeXmlComments(filePath);
                }
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ethos");
                });
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseMiddleware<ExceptionHandler>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
