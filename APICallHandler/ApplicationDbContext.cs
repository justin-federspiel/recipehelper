using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;

namespace APICallHandler
{
    public class ApplicationDbContext : DbContext { 

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }

        public ApplicationDbContext()
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            if (!optionsBuilder.IsConfigured)
            {

                IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
                var connectionString = configuration.GetConnectionString("MainDatabaseEdit");
                optionsBuilder.UseSqlServer(connectionString);
                
            }
        } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            RegisterMaps(modelBuilder);
        }        
        public DbSet<Cook> Cooks { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientTag> IngredientTags { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<RecipeTag> RecipeTags { get; set; }
        public DbSet<Tag> Tags { get; set; }        

        private static void RegisterMaps(ModelBuilder builder)
        {
            new CookMap(builder);
            new IngredientMap(builder);
            new IngredientTagMap(builder);
            new RecipeMap(builder);
            new RecipeIngredientMap(builder);
            new RecipeTagMap(builder);
            new TagMap(builder);
        }
    }
}
