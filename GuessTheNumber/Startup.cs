using System;
using System.IO;
using GuessTheNumber.Configuration;
using GuessTheNumber.Models;
using GuessTheNumber.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace GuessTheNumber
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
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                    options.SerializerSettings.MaxDepth = 1;
                });
            
            services
                .AddRouting()
                .AddCors()
                .AddHealthChecks();
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("GuessNumbersDb"));

            services.Configure<NumbersSettings>(Configuration.GetSection("NumbersSettings"));
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Guess The Number API", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "GuessTheNumber.xml"));
            });

            services.AddScoped<INumberDataRecordsRepository, NumberDataRecordsRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.DisplayRequestDuration();
                c.DisplayOperationId();
                c.RoutePrefix = "explore";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Guess The Number API V1");
            });
            
            app.UseHealthChecks("/");

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}