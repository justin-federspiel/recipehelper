using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICallHandler;
using Authentication;
using Microsoft.EntityFrameworkCore;

namespace WebListener
{
    public class Startup
    {
        //private DbContextOptions<ApplicationDbContext> Options;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = "use your own, but it's better to grab this from either the command line, and environment variable, or a config file that isn't committed to a public repo.";
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {                
                CookAPI.MapAPIEndpoints(endpoints);
                RecipeAPI.MapAPIEndpoints(endpoints);
                IngredientAPI.MapAPIEndpoints(endpoints);
                TagAPI.MapAPIEndpoints(endpoints);
                IngredientTagAPI.MapAPIEndpoints(endpoints);
            });
        }
    }
}
