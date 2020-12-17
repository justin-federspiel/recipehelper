using Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICallHandler
{
    public class CookMap : IEntityMap
    {
        public CookMap(ModelBuilder builder)
        {
            builder.Entity<Cook>(a => { a.HasKey(x => x.Id); });
        }
    }

    public class IngredientMap : IEntityMap
    {
        public IngredientMap(ModelBuilder builder)
        {
            builder.Entity<Ingredient>(a => { a.HasKey(x => x.Id); });
        }
    }

    public class IngredientTagMap : IEntityMap
    {
        public IngredientTagMap(ModelBuilder builder)
        {
            builder.Entity<IngredientTag>().HasKey(a => new { a.Id });
            builder.Entity<IngredientTag>().HasOne<Ingredient>(it => it.Ingredient)
                .WithMany(i => i.IngredientTags)
                .HasForeignKey(it => it.IngredientId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<IngredientTag>().HasOne<Tag>(it => it.Tag)
                .WithMany(t => t.IngredientTags)
                .HasForeignKey(it => it.TagId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<IngredientTag>().HasOne<Cook>(it => it.Cook)
                .WithMany(c => c.IngredientTags)
                .HasForeignKey(it => it.CookId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class RecipeMap : IEntityMap
    {
        public RecipeMap(ModelBuilder builder)
        {
            builder.Entity<Recipe>(r => { r.HasKey(x => x.Id); });
            builder.Entity<Recipe>()
                .HasOne<Cook>(r => r.Cook)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CookId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class RecipeIngredientMap: IEntityMap
    {
        public RecipeIngredientMap(ModelBuilder builder)
        {
            builder.Entity<RecipeIngredient>().HasKey(ri => new { ri.RecipeId, ri.IngredientId, ri.Measurement, ri.Preparation });
            builder.Entity<RecipeIngredient>().HasOne<Recipe>(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<RecipeIngredient>().HasOne<Ingredient>(ri => ri.Ingredient)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<RecipeIngredient>().OwnsOne<Fractionable>(ri => ri.Quantity);
        }
    }

    public class RecipeTagMap : IEntityMap
    {
        public RecipeTagMap(ModelBuilder builder)
        {
            builder.Entity<RecipeTag>().HasKey(x => new { x.Id });
            builder.Entity<RecipeTag>().HasOne<Recipe>(rt => rt.Recipe)
                .WithMany(r => r.RecipeTags)
                .HasForeignKey(rt => rt.RecipeId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<RecipeTag>().HasOne<Tag>(rt => rt.Tag)
                .WithMany(t => t.RecipeTags)
                .HasForeignKey(rt => rt.TagId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class TagMap : IEntityMap
    {
        public TagMap(ModelBuilder builder)
        {
            builder.Entity<Tag>().HasKey(a => new { a.Id });            
        }
    }
}
