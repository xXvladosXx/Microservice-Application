using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
    public class Startup
    {
        private readonly IWebHostEnvironment _enviroment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _enviroment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            if (_enviroment.IsProduction())
            {
                Console.WriteLine("using sqlserver db");
                services.AddDbContext<AppDbContext>(opt =>
                {
                   opt.UseSqlServer(Configuration.GetConnectionString("Platforms"));
                });
            }
            else
            {
                Console.WriteLine("using inmem db");
                services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMemory"));
            }
            
            services.AddScoped<IPlatformRepo, PlatfromRepo>();
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });
            
            Console.WriteLine($" Coomdn {Configuration["CommandService"]}");
        }

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            PrepDb.PrepPopulation(app, env.IsProduction());
        }
    }
}
