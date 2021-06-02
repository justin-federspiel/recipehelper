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
using Microsoft.Extensions.Configuration;

namespace WebListener
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public IConfiguration Configuration { get; }
        //private DbContextOptions<ApplicationDbContext> Options;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {            
            //string connectionString = "";
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MainDatabaseEdit")));
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
                ShoppingListAPI.MapAPIEndpoints(endpoints);
            });
        }
    }
}
