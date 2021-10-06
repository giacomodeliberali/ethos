using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ethos.Application;
using Ethos.Application.Contracts;
using Ethos.Domain.Entities;
using Ethos.EntityFrameworkCore;
using Ethos.Shared;
using Ethos.Web.Host.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            // register strongly typed configuration
            services.Configure<JwtConfig>(_configuration.GetSection(nameof(JwtConfig)));
            services.Configure<EmailConfig>(_configuration.GetSection(nameof(EmailConfig)));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("Default"));
            });

            services.AddEthosIdentity(_configuration);

            // add application module
            services.AddApplicationModule();
            services.AddRepositories();
            services.AddQueries();

            // add controllers
            services.AddEthosControllers();

            services.AddEthosSwagger();

            services.AddCors(o => o.AddPolicy("DevCorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
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

                app.UseCors("DevCorsPolicy");
            }

            // app.UseHttpsRedirection();
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
