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
    public class TagAPI
    {
        private readonly ApplicationDbContext _context; //Should this have an ApplicationDbContext?  Shouldn't this be dealt with elsewhere?        

        public TagAPI(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<Tag[]> GetMine(AuthenticationToken user, int page = 0, int count = 100)
        {
            var t = _context.Tags
                .Where(t => t.IngredientTags.Any(it => it.CookId == user.ApplicationWideId) || t.RecipeTags.Any(rt => rt.Recipe.CookId == user.ApplicationWideId))
                .Skip(page * count)
                .Take(count);
            Tag[] result = await t.ToArrayAsync();
            return result;
        }

        public static void MapAPIEndpoints(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/tag/mine/", async (context) => {
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
                        using ApplicationDbContext ctx = new ApplicationDbContext();
                        AuthenticationToken tokenUser = new AuthenticationToken { ApplicationWideId = id, ApplicationWideName = (context.Request.Query.ContainsKey("name")) ? context.Request.Query["name"].ToString() : "" };
                        TagAPI api = new TagAPI(ctx);
                        await context.Response.WriteAsJsonAsync(api.GetMine(tokenUser, page, count));
                    }
                }
            });
        }
    }
}
