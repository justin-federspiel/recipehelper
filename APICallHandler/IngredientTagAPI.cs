using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Models;

namespace APICallHandler
{
    public class IngredientTagAPI
    {
        private readonly ApplicationDbContext _context;

        public IngredientTagAPI(ApplicationDbContext context) {
            this._context = context;
        }

        public async Task<IngredientTag[]> GetMyIngredientTags(AuthenticationToken user, int page = 0, int count = 100)
        {
            Cook me;
            if(user.ApplicationWideId != 0)
            {
                me = await new CookAPI().GetOne(user, user.ApplicationWideId);
            } else
            {
                me = await new CookAPI().GetOneByName(user, user.ApplicationWideName);
            }
            if(me == null)
            {
                return Array.Empty<IngredientTag>();
            }

            var t = _context.IngredientTags.Where(it => it.CookId == me.Id)
                .Include(it => it.Tag)
                .Include(it => it.Ingredient)
                .Include(it => it.Cook)
                .Select(e => new IngredientTag { 
                    IngredientId = e.IngredientId, 
                    Ingredient = new Ingredient { Name = e.Ingredient.Name, Id = e.Ingredient.Id }, 
                    TagId = e.TagId, 
                    Tag = new Tag { Text = e.Tag.Text, Id = e.Tag.Id },
                    CookId = e.CookId,
                    Cook = new Cook { Name = e.Cook.Name, Id = e.Cook.Id }
                    })
                .Skip(page * count)
                .Take(count);
            IngredientTag[] result = await t.ToArrayAsync();            
            return result;
        }

        public static void MapAPIEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/ingredient-tag/mine", async (context) =>
            {
                long id = 0;
                if ((!context.Request.Query.ContainsKey("name") && !context.Request.Query.ContainsKey("id")) ||
                (context.Request.Query.ContainsKey("name") && string.IsNullOrWhiteSpace(context.Request.Query["name"].ToString()) ||
                (context.Request.Query.ContainsKey("id") && !long.TryParse(context.Request.Query["id"].ToString(), out id))))
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 400, Message = "Either both no name and no id specified for that Cook, or one of the two wasn't readable." });
                } else
                {
                    int page = 0, count = 100;
                    if(context.Request.Query.ContainsKey("page") && !int.TryParse(context.Request.Query["page"].ToString(), out page) ||
                    (context.Request.Query.ContainsKey("count") && !int.TryParse(context.Request.Query["count"].ToString(), out count)) ||
                    (context.Request.Query.ContainsKey("page") && !context.Request.Query.ContainsKey("count"))) {
                        await context.Response.WriteAsJsonAsync(new { ResponseCode = 400, Message = "Either couldn't read page or count requested, or page was requested and count (per page) wasn't." });
                    } else
                    {
                        using ApplicationDbContext ctx = new ApplicationDbContext();
                        AuthenticationToken tokenUser = new AuthenticationToken { ApplicationWideId = id, ApplicationWideName = (context.Request.Query.ContainsKey("name")) ? context.Request.Query["name"].ToString() : "" };                        
                        IngredientTagAPI api = new IngredientTagAPI(ctx);
                        await context.Response.WriteAsJsonAsync(api.GetMyIngredientTags(tokenUser, page, count));
                    }
                }

            });
        }
    }
}
