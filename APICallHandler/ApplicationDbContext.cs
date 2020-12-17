using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
                string connectionString = "Data Source=DESKTOP-C5N5TVS;Initial Catalog=BlazorLearner;Integrated Security=True;Pooling=False"; // was Configuration.GetConnectionString("DefaultConnection")
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
