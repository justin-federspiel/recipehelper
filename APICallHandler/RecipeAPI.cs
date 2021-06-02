using System.IO;
using System.Collections.Generic;
using System.Linq;
using Inflector;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Authentication;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System;

namespace APICallHandler
{
    public class RecipeAPI
    {
        public RecipeAPI() { }

        public async Task<Recipe[]> GetAll(AuthenticationToken user, int page = 0, int count = 100)
        {
            using ApplicationDbContext context = new ApplicationDbContext();
            var t = context.Recipes                
                .Skip(page * count)
                .Take(count);
            Recipe[] result = await t.ToArrayAsync();
            return result;
        }

        public async Task<Recipe> GetOne(AuthenticationToken user, long id = 0)
        {
            using ApplicationDbContext context = new ApplicationDbContext();
            Recipe result = await context.Recipes.Where(c => c.Id == id).Select(r => 
            new Recipe { 
                Id = r.Id, 
                Cook = r.Cook, 
                CookId = r.CookId, 
                Name = r.Name, 
                MainGraphicLink = r.MainGraphicLink, 
                Instructions = r.Instructions,
                RecipeTags = (ICollection<RecipeTag>)r.RecipeTags.Select(rt => new RecipeTag { RecipeId = rt.RecipeId, TagId = rt.TagId, Tag = new Tag { Id = rt.Tag.Id, Text = rt.Tag.Text } }),
                RecipeIngredients = (ICollection<RecipeIngredient>)r.RecipeIngredients.Select(ri => new RecipeIngredient
                {
                    RecipeId = ri.RecipeId,
                    IngredientId = ri.IngredientId,                    
                    Ingredient = new Ingredient
                    {
                        Id = ri.Ingredient.Id,
                        Name = ri.Ingredient.Name,
                        IngredientTags = (ICollection<IngredientTag>)ri.Ingredient.IngredientTags.Where(riiit => riiit.CookId == ri.Recipe.CookId)
                        .Select(it => new IngredientTag
                        {
                            Id = it.Id,
                            CookId = it.CookId,
                            IngredientId = it.IngredientId,
                            TagId = it.TagId,
                            Tag = new Tag
                            {
                                Id = it.Tag.Id,
                                Text = it.Tag.Text
                            }
                        })
                    },
                    Quantity = new Fractionable
                    {
                        Whole = ri.Quantity.Whole,
                        Numerator = ri.Quantity.Numerator,
                        Denominator = ri.Quantity.Denominator
                    },
                    Measurement = ri.Measurement,
                    Preparation = ri.Preparation
                })                 
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Recipe[]> GetMine(AuthenticationToken user, int page = 0, int count = 100)
        {
            using ApplicationDbContext context = new ApplicationDbContext();
            var t = context.Recipes
                .Where(r => r.CookId == user.ApplicationWideId)
                .Select(r => new Recipe
                {
                    Id = r.Id,
                    CookId = r.CookId,
                    Name = r.Name,
                    Instructions = r.Instructions,
                    MainGraphicLink = r.MainGraphicLink,
                    RecipeTags = (ICollection<RecipeTag>)r.RecipeTags.Select(rt => new RecipeTag { RecipeId = rt.RecipeId, TagId = rt.TagId, Tag = new Tag { Id = rt.TagId, Text = rt.Tag.Text } }),
                    RecipeIngredients = (ICollection<RecipeIngredient>)r.RecipeIngredients.Select(ri => new RecipeIngredient
                    {                        
                        RecipeId = ri.RecipeId,
                        IngredientId = ri.IngredientId,
                        Ingredient = new Ingredient
                        {
                            Id = ri.Ingredient.Id,
                            Name = ri.Ingredient.Name,
                            IngredientTags = (ICollection<IngredientTag>)ri.Ingredient.IngredientTags.Where(riiit => riiit.CookId == user.ApplicationWideId)
                            .Select(it => new IngredientTag {
                                Id = it.Id,
                                CookId = it.CookId, 
                                IngredientId = it.IngredientId, 
                                TagId = it.TagId, 
                                Tag = new Tag { 
                                    Id = it.TagId, 
                                    Text = it.Tag.Text 
                                } 
                            })
                        },
                        Quantity = new Fractionable { 
                            Whole = ri.Quantity.Whole,
                            Numerator = ri.Quantity.Numerator,
                            Denominator = ri.Quantity.Denominator
                        },
                        Measurement = ri.Measurement,
                        Preparation = ri.Preparation
                    })
                })
                .Skip(page * count)
                .Take(count);
            Recipe[] result = await t.ToArrayAsync();
            return result;
        }

        public async Task<int> Update(AuthenticationToken user, Recipe updatedInfo)
        {            
            using (ApplicationDbContext deletionContext = new ApplicationDbContext())
            {
                deletionContext.RemoveRange(deletionContext.RecipeIngredients.Where(ri => ri.RecipeId == updatedInfo.Id));
                deletionContext.RemoveRange(deletionContext.RecipeTags.Where(rt => rt.RecipeId == updatedInfo.Id));
                await deletionContext.SaveChangesAsync();
            }
            using ApplicationDbContext updateContext = new ApplicationDbContext();
            Recipe updateMe = await updateContext.Recipes.FirstOrDefaultAsync(a => a.Id == updatedInfo.Id);
            if (updateMe == null) throw new ApplicationException("Could not find a Recipe with that Id.");
            updateMe.CookId = updatedInfo.CookId;
            updateMe.Name = updatedInfo.Name ?? updateMe.Name ?? "";
            updateMe.Instructions = updatedInfo.Instructions ?? updateMe.Instructions ?? "";
            updateMe.MainGraphicLink = updatedInfo.MainGraphicLink ?? updateMe.MainGraphicLink ?? "";
            if (updatedInfo.RecipeIngredients != null && updatedInfo.RecipeIngredients.Count > 0)
            {
                List<RecipeIngredient> thisRecipesIngredients = updatedInfo.RecipeIngredients.ToList();
                for (int index = 0; index < thisRecipesIngredients.Count; index++)
                {
                    RecipeIngredient ri = thisRecipesIngredients[index];
                    using(ApplicationDbContext ingredientTaggingContext = new ApplicationDbContext())
                    {
                        if (ri.Ingredient != null && ri.Ingredient.IngredientTags != null)
                        {
                            IEnumerator<IngredientTag> ite = ri.Ingredient.IngredientTags.GetEnumerator();                            
                            for(;ite.MoveNext();)
                            {
                                IngredientTag it = ite.Current;
                                if (ite.Current.CookId == null || ite.Current.CookId != updateMe.CookId) ite.Current.CookId = updateMe.CookId;
                                if(it.Tag != null)
                                {
                                    Tag maybeImATagInTheDatabase = await ingredientTaggingContext.Tags.Where(t => it.Tag.Text == t.Text || (it.Tag.Id != null && t.Id == it.Tag.Id)).FirstOrDefaultAsync<Tag>();
                                    if(maybeImATagInTheDatabase != null)
                                    {
                                        it.TagId = maybeImATagInTheDatabase.Id;
                                    } else
                                    {
                                        ingredientTaggingContext.Add(it.Tag);
                                        await ingredientTaggingContext.SaveChangesAsync();
                                        it.TagId = it.Tag.Id;
                                        ite.Current.TagId = it.TagId;
                                    }
                                }
                            }
                        }
                    }
                    Ingredient maybeIExistInTheDatabase = await updateContext.Ingredients.Where(i => ri.IngredientId == i.Id || (ri.Ingredient != null && ri.Ingredient.Name == i.Name)).FirstOrDefaultAsync<Ingredient>();
                    if (maybeIExistInTheDatabase != null)
                    {
                        ri.Ingredient = null;
                        ri.IngredientId = maybeIExistInTheDatabase.Id;
                    }
                }
                updateMe.RecipeIngredients = RemoveDuplicateIngredients(thisRecipesIngredients);
            }

            if (updatedInfo.RecipeTags != null && updatedInfo.RecipeTags.Count > 0)
            {
                List<RecipeTag> thisRecipesTags = updatedInfo.RecipeTags.ToList();
                for (int index = 0; index < thisRecipesTags.Count; index++)
                {
                    RecipeTag rt = thisRecipesTags[index];
                    if (rt.Tag != null)
                    {
                        Tag maybeImATagInTheDatabase = await updateContext.Tags.Where(t => rt.Tag.Text == t.Text || (rt.Tag.Id != null && t.Id == rt.Tag.Id)).FirstOrDefaultAsync<Tag>();
                        if (maybeImATagInTheDatabase != null)
                        {
                            rt.Tag = null;
                            rt.TagId = maybeImATagInTheDatabase.Id;
                        }
                        else
                        {
                            updateContext.Tags.Add(rt.Tag);
                            await updateContext.SaveChangesAsync(); //Adding to the context and saving will give it an Id..
                            thisRecipesTags[index].TagId = rt.Tag.Id;
                            thisRecipesTags[index].Tag.Id = rt.Tag.Id;
                            rt.Tag = null;

                        }
                    }
                    else if (rt.TagId != null)
                    {
                        Tag maybeImATagInTheDatabase = await updateContext.Tags.SingleOrDefaultAsync<Tag>(t => rt.TagId == t.Id);
                        if (maybeImATagInTheDatabase == null)
                        {
                            throw new ApplicationException("Cannot find a Tag with that Id in the data.");
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Tag specified, but neither Text nor Id were mentioned.");
                    }
                }
                updateMe.RecipeTags = RemoveDuplicateTags(thisRecipesTags);
            }
            
            updateContext.Update(updateMe);            
            return await updateContext.SaveChangesAsync();
        }

        public async Task<int> Create(AuthenticationToken user, Recipe newInfo)
        {
            using ApplicationDbContext context = new ApplicationDbContext();
            Recipe makeMe = newInfo;
            if(makeMe.RecipeIngredients != null && makeMe.RecipeIngredients.Count > 0)
            {
                List<RecipeIngredient> thisRecipesIngredients = makeMe.RecipeIngredients.ToList();
                for(int index = 0; index < thisRecipesIngredients.Count; index++)
                {
                    RecipeIngredient ri = thisRecipesIngredients[index];
                    Ingredient maybeIExistInTheDatabase = await context.Ingredients.Where(i => ri.IngredientId == i.Id || (ri.Ingredient != null && ri.Ingredient.Name == i.Name)).FirstOrDefaultAsync<Ingredient>();
                    if (maybeIExistInTheDatabase != null) {
                        ri.Ingredient = null;
                        ri.IngredientId = maybeIExistInTheDatabase.Id;
                    }
                }
                makeMe.RecipeIngredients = RemoveDuplicateIngredients(thisRecipesIngredients);
            }
            
            if (makeMe.RecipeTags != null &&  makeMe.RecipeTags.Count > 0)
            {
                List<RecipeTag> thisRecipesTags = makeMe.RecipeTags.ToList();
                for (int index = 0; index < thisRecipesTags.Count; index++)
                {
                    RecipeTag rt = thisRecipesTags[index];
                    if (rt.Tag != null)
                    {
                        Tag maybeImATagInTheDatabase = await context.Tags.Where(t => rt.Tag.Text == t.Text || (rt.Tag.Id != null && t.Id == rt.Tag.Id)).FirstOrDefaultAsync<Tag>();
                        if (maybeImATagInTheDatabase != null)
                        {
                            rt.Tag = null;
                            rt.TagId = maybeImATagInTheDatabase.Id;
                        }
                        else
                        {
                            context.Tags.Add(rt.Tag);
                            await context.SaveChangesAsync(); //Adding to the context and saving will give it an Id..
                            thisRecipesTags[index].TagId = rt.Tag.Id;
                            thisRecipesTags[index].Tag.Id = rt.Tag.Id;
                            rt.Tag = null;

                        }
                    } else if (rt.TagId != null)
                    {
                        Tag maybeImATagInTheDatabase = await context.Tags.SingleOrDefaultAsync<Tag>(t => rt.TagId == t.Id);
                        if (maybeImATagInTheDatabase == null)
                        {
                            throw new ApplicationException("Cannot find a Tag with that Id in the data.");
                        }
                    } else
                    {
                        throw new ApplicationException("Tag specified, but neither Text nor Id were mentioned.");
                    }
                }
                makeMe.RecipeTags = RemoveDuplicateTags(thisRecipesTags);                
            }
            context.Recipes.Add(makeMe);
            int numberCreated = await context.SaveChangesAsync();
            return numberCreated;
        }

        private static List<RecipeIngredient> RemoveDuplicateIngredients(List<RecipeIngredient> recipeIngredients)
        {
            List<RecipeIngredient> result = new List<RecipeIngredient>();
            for(int index = 0; index < recipeIngredients.Count; index++) 
            {
                RecipeIngredient mightBeADuplicate = recipeIngredients[index];
                RecipeIngredient original = result.FirstOrDefault(i => RecipeIngredient.SameModelIdentification(i, mightBeADuplicate));
                if(original != null)
                {
                    original.Quantity += mightBeADuplicate.Quantity;
                } else
                {
                    result.Add(mightBeADuplicate);
                }
            }
            return result;
        }

        private static List<RecipeTag> RemoveDuplicateTags(List<RecipeTag> recipeTags)
        {
            List<RecipeTag> result = new List<RecipeTag>();
            for (int index = 0; index < recipeTags.Count; index++)
            {
                RecipeTag mightBeADuplicate = recipeTags[index];
                RecipeTag original = result.FirstOrDefault(i => RecipeTag.SameModelIdentification(i, mightBeADuplicate));
                if (original == null)
                {                    
                    result.Add(mightBeADuplicate);
                }
            }
            return result;
        }

        public async Task<int> Delete(AuthenticationToken user, long id = 0)
        {
            using ApplicationDbContext context = new ApplicationDbContext();
            Recipe deleteMe = new Recipe { Id = id };
            context.Recipes.Remove(deleteMe);
            int numberDeleted = await context.SaveChangesAsync();
            return numberDeleted;
        }

        public static void MapAPIEndpoints(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/recipe/all/", async (context) =>
            {
                if (!context.Request.Query.ContainsKey("page") ||
                !int.TryParse(context.Request.Query["page"].ToString(), out int page)) page = 0;
                if (!context.Request.Query.ContainsKey("count") ||
                !int.TryParse(context.Request.Query["count"].ToString(), out int count)) count = 100;
                RecipeAPI api = new RecipeAPI();
                await context.Response.WriteAsJsonAsync(api.GetAll(new AuthenticationToken(), page, count));
            });
            endpoints.MapGet("/api/recipe/", async (context) =>
            {
                if (!context.Request.Query.ContainsKey("id") ||
                !long.TryParse(context.Request.Query["id"].ToString(), out long id))
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 404, Message = "Could not find a Recipe with that ID." });
                }
                else
                {                    
                    RecipeAPI api = new RecipeAPI();
                    await context.Response.WriteAsJsonAsync(api.GetOne(new AuthenticationToken(), id));
                }
            });
            endpoints.MapGet("/api/recipe/mine/", async (context) => {
                long id = 0;
                if ((!context.Request.Query.ContainsKey("name") && !context.Request.Query.ContainsKey("id")) ||
                (context.Request.Query.ContainsKey("name") && string.IsNullOrWhiteSpace(context.Request.Query["name"].ToString()) ||
                (context.Request.Query.ContainsKey("id") && !long.TryParse(context.Request.Query["id"].ToString(), out id))))
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 400, Message = "Either both no name and no id specified for that Cook, or one of the two wasn't readable." });
                }
                else
                {
                    int page = 0, count = 100;
                    if (context.Request.Query.ContainsKey("page") && !int.TryParse(context.Request.Query["page"].ToString(), out page) ||
                    (context.Request.Query.ContainsKey("count") && !int.TryParse(context.Request.Query["count"].ToString(), out count)) ||
                    (context.Request.Query.ContainsKey("page") && !context.Request.Query.ContainsKey("count")))
                    {
                        await context.Response.WriteAsJsonAsync(new { ResponseCode = 400, Message = "Either couldn't read page or count requested, or page was requested and count (per page) wasn't." });
                    }
                    else
                    {                        
                        AuthenticationToken tokenUser = new AuthenticationToken { ApplicationWideId = id, ApplicationWideName = (context.Request.Query.ContainsKey("name")) ? context.Request.Query["name"].ToString() : "" };
                        RecipeAPI api = new RecipeAPI();
                        await context.Response.WriteAsJsonAsync(api.GetMine(tokenUser, page, count));
                    }
                }
            });
            endpoints.MapDelete("/api/recipe", async (context) =>
            {
                if (!context.Request.Query.ContainsKey("id") ||
                !long.TryParse(context.Request.Query["id"].ToString(), out long id))
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 404, Message = "Could not find a Recipe with that ID." });
                }
                else
                {                    
                    RecipeAPI api = new RecipeAPI();
                    await context.Response.WriteAsJsonAsync(api.Delete(new AuthenticationToken(), id));
                }
            });
            endpoints.MapPost("/api/recipe", async (context) =>
            {
                Stream body = context.Request.Body;
                Recipe recipe = null;                
                if(body != null)
                {
                    string bodyJsonString = await (new StreamReader(body)).ReadToEndAsync();
                    recipe = JsonSerializer.Deserialize<Recipe>(bodyJsonString);
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 400, Message = "Body of request must contain a Recipe in Json format." });
                }
                if (recipe != null)
                {                    
                    RecipeAPI api = new RecipeAPI();
                    await context.Response.WriteAsJsonAsync(api.Create(new AuthenticationToken(), recipe));
                } else
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 400, Message = "Could not read a Recipe from the body of your request." });
                }
                
            });
            endpoints.MapPut("/api/recipe", async (context) => {
                Stream body = context.Request.Body;
                Recipe recipe = null;
                if (body != null)
                {
                    string bodyJsonString = await (new StreamReader(body)).ReadToEndAsync();
                    recipe = JsonSerializer.Deserialize<Recipe>(bodyJsonString);
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 400, Message = "Body of request must contain a Recipe in Json format." });
                }
                if (recipe != null)
                {                    
                    RecipeAPI api = new RecipeAPI();
                    await context.Response.WriteAsJsonAsync(api.Update(new AuthenticationToken(), recipe));
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 400, Message = "Could not read a Recipe from the body of your request." });
                }
            });
        }
    }
}
