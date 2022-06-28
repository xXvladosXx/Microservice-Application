using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder,
            bool isProd)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static void SeedData(AppDbContext context, bool prod)
        {
            if (prod)
            {
                Console.WriteLine("migrating");

                try
                {
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            
            if (!context.Platforms.Any())
            {
                Console.WriteLine("Seeding data");
                context.Platforms.AddRange(
                    new Platform() {Name = "dot net", Publisher = "Microsoft", Cost = "Free"},
                    new Platform() {Name = "sql express", Publisher = "Microsoft", Cost = "Free"},
                    new Platform() {Name = "kubernetes", Publisher = "CNCF", Cost = "Free"}
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("We have data");
            }
        }
    }
}