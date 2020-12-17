using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Authentication;
using Models;
using Microsoft.EntityFrameworkCore;

namespace APICallHandler
{
    public class CookAPI
    {

        private readonly ApplicationDbContext _context; //Should this have an ApplicationDbContext?  Shouldn't this be dealt with elsewhere?

        public CookAPI(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<Cook[]> GetAll(AuthenticationToken user, int page = 0, int count = 100)
        {
            var t =  _context.Cooks                
                .Skip(page * count)
                .Take(count);
            Cook[] result = await t.ToArrayAsync();
            return result;
        }

        public async Task<Cook> GetOne(AuthenticationToken user, long id = 0)
        {
            Cook result = await _context.Cooks.SingleOrDefaultAsync(c => c.Id == id);
            return result;
        }

        public async Task<Cook> GetOneByName(AuthenticationToken user, string name = "")
        {
            Cook result = await _context.Cooks.Where(c => c.Name == name).FirstOrDefaultAsync<Cook>();
            return result;
        }

        public async Task<int> Update(AuthenticationToken user, long id = 0, string name = "")
        {
            Cook updateMe = new Cook { Id = id, Name = name };
            _context.Cooks.Update(updateMe);
            int numberUpdated = await _context.SaveChangesAsync();
            return numberUpdated;
        }

        public async Task<int> Create(AuthenticationToken user, string name = "")
        {
            Cook makeMe = new Cook { Name = name };
            _context.Cooks.Add(makeMe);
            int numberCreated = await _context.SaveChangesAsync();
            return numberCreated;
        }

        public async Task<int> Delete(AuthenticationToken user, long id = 0)
        {
            Cook deleteMe = new Cook { Id = id };
            _context.Cooks.Remove(deleteMe);
            int numberDeleted = await _context.SaveChangesAsync();
            return numberDeleted;
        }
        
        public static void MapAPIEndpoints(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
            endpoints.MapGet("/api/cook/all/", async (context) =>
            {
                if (!context.Request.Query.ContainsKey("page") ||
                !int.TryParse(context.Request.Query["page"].ToString(), out int page)) page = 0;
                if (!context.Request.Query.ContainsKey("count") ||
                !int.TryParse(context.Request.Query["count"].ToString(), out int count)) count = 100;
                using ApplicationDbContext ctx = new ApplicationDbContext();
                CookAPI api = new CookAPI(ctx);
                Models.Cook[] cooks = await api.GetAll(new AuthenticationToken(), page, count);
                await context.Response.WriteAsJsonAsync(api.GetAll(new AuthenticationToken(), page, count));
            });
            endpoints.MapGet("/api/cook/", async context =>
            {
                if (!context.Request.Query.ContainsKey("id") ||
                !long.TryParse(context.Request.Query["id"].ToString(), out long id))
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 404, Message = "Could not find a Cook with that ID." });
                }
                else
                {
                    using ApplicationDbContext ctx = new ApplicationDbContext();
                    CookAPI api = new CookAPI(ctx);
                    await context.Response.WriteAsJsonAsync(api.GetOne(new AuthenticationToken(), id));
                }
            });
            endpoints.MapDelete("/api/cook", async context =>
            {
                if (!context.Request.Query.ContainsKey("id") ||
                !long.TryParse(context.Request.Query["id"].ToString(), out long id))
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 404, Message = "Could not find a Cook with that ID." });
                }
                else
                {
                    using ApplicationDbContext ctx = new ApplicationDbContext();
                    CookAPI api = new CookAPI(ctx);
                    await context.Response.WriteAsJsonAsync(api.Delete(new AuthenticationToken(), id));
                }
            });
            endpoints.MapPost("/api/cook", async context =>
            {
                if (!context.Request.Query.ContainsKey("name") ||
                string.IsNullOrWhiteSpace(context.Request.Query["name"].ToString()))
                {
                    await context.Response.WriteAsJsonAsync(new { ResponseCode = 404, Message = "Could not create a Cook with no name." });
                }
                else
                {
                    var name = context.Request.Query["name"].ToString();
                    using ApplicationDbContext ctx = new ApplicationDbContext();
                    CookAPI api = new CookAPI(ctx);
                    await context.Response.WriteAsJsonAsync(api.Create(new AuthenticationToken(), name));
                }
            });
        }
    }
}
