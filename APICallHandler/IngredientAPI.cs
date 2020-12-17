using Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICallHandler
{
    public class IngredientAPI
    {
        private readonly ApplicationDbContext _context; //Should this have an ApplicationDbContext?  Shouldn't this be dealt with elsewhere?
        private static readonly int DEFAULT_PARTIAL_MATCH_COUNT = 8;
        private static readonly int MAX_PARTIAl_MATCH_COUNT = 100;

        public IngredientAPI(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<Ingredient[]> GetAll(AuthenticationToken user, int page = 0, int count = 100)
        {
            var t = _context.Ingredients
                .Skip(page * count)
                .Take(count);
            Ingredient[] result = await t.ToArrayAsync();
            return result;
        }

        public async Task<Ingredient[]> GetMatches(AuthenticationToken user, string partial, bool? mustBeginWithPartial, int count = 8)
        {
            var i = _context.Ingredients
                .Where(a => (mustBeginWithPartial ?? true) ? a.Name.StartsWith(partial) : a.Name.Contains(partial))
                .Take(count);
            Ingredient[] result = await i.ToArrayAsync();
            return result;
        }

        public async Task<Ingredient> GetOne(AuthenticationToken user, long id = 0)
        {
            Ingredient result = await _context.Ingredients.SingleOrDefaultAsync(c => c.Id == id);
            return result;
        }

        public async Task<object> Create(AuthenticationToken user, string name = "")
        {
            Ingredient makeMe = new Ingredient { Name = name };
            bool alreadyExists = (await _context.Ingredients.Where(i => i.Name == name).FirstOrDefaultAsync<Ingredient>() != null);
            if (alreadyExists)
            {
                return new { ResponseCode = 400, Message = "Ingredient already exists and cannot be created again." };
            }
            
            _context.Ingredients.Add(makeMe);
            int numberCreated = await _context.SaveChangesAsync();
            return numberCreated;
        }

        public static void MapAPIEndpoints(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/ingredient/all", async (context) =>
            {
                if (!context.Request.Query.ContainsKey("page") ||
                    !int.TryParse(context.Request.Query["page"].ToString(), out int page)) page = 0;
                if (!context.Request.Query.ContainsKey("count") ||
                !int.TryParse(context.Request.Query["count"].ToString(), out int count)) count = 100;
                using ApplicationDbContext ctx = new ApplicationDbContext();
                IngredientAPI api = new IngredientAPI(ctx);
                Models.Ingredient[] ingredients = await api.GetAll(new AuthenticationToken(), page, count);
                await context.Response.WriteAsJsonAsync(ingredients);
            }
            );
            endpoints.MapGet("/api/ingredient/", async (context) =>
            {
                if (!context.Request.Query.ContainsKey("id") ||
                !long.TryParse(context.Request.Query["id"].ToString(), out long id))
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 404, Message = "Could not find an Ingredient with that ID." });
                }
                else
                {
                    using ApplicationDbContext ctx = new ApplicationDbContext();
                    IngredientAPI api = new IngredientAPI(ctx);
                    await context.Response.WriteAsJsonAsync(api.GetOne(new AuthenticationToken(), id));
                }
            });

            endpoints.MapGet("/api/ingredient/match", async (context) =>
            {
                if(!context.Request.Query.ContainsKey("partial"))
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 400, Message = "No partial provided to match Ingredients with." });
                } else
                {
                    string partial = context.Request.Query["partial"].ToString();
                    bool mustBeginWithPartial = !context.Request.Query.ContainsKey("starts");
                    if (!mustBeginWithPartial) bool.TryParse(context.Request.Query["starts"].ToString(), out mustBeginWithPartial);
                    int count = DEFAULT_PARTIAL_MATCH_COUNT;
                    if(context.Request.Query.ContainsKey("count"))
                    { //count is optional, but, we won't support abusing it
                        if (!int.TryParse(context.Request.Query["count"], out count) || count > MAX_PARTIAl_MATCH_COUNT || count < 1)
                        {
                            await context.Response.WriteAsJsonAsync(new { ResponseCode = 400, Message = "Invalid Ingredient count specified in request." });
                        }
                    }
                    using ApplicationDbContext ctx = new ApplicationDbContext();
                    IngredientAPI api = new IngredientAPI(ctx);
                    await context.Response.WriteAsJsonAsync(api.GetMatches(new AuthenticationToken(), partial, mustBeginWithPartial, count));
                }
            });

            endpoints.MapPost("/api/ingredient", async context =>
            {
                if (!context.Request.Query.ContainsKey("name") ||
                string.IsNullOrWhiteSpace(context.Request.Query["name"].ToString()))
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 404, Message = "Could not create an Ingredient with no name." });
                }
                else
                {
                    var name = context.Request.Query["name"].ToString();
                    using ApplicationDbContext ctx = new ApplicationDbContext();
                    IngredientAPI api = new IngredientAPI(ctx);
                    await context.Response.WriteAsJsonAsync(api.Create(new AuthenticationToken(), name));
                }
            });


        }
    }
}
