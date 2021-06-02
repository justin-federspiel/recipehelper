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
    public class ShoppingListAPI
    {

        public static void MapAPIEndpoints(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/shoppinglist", async (context) =>
            {
                if(context.Request.Query.ContainsKey("recipes")) {
                    string recipeIDparameter;
                    recipeIDparameter = context.Request.Query["recipes"].ToString();
                    string[] recipeIDs = recipeIDparameter.Split(",");
                    List<long> recipeIDList = new List<long>();
                    foreach(string id in recipeIDs)
                    {
                        long addMe = 0;
                        if(long.TryParse(id, out addMe))
                        {
                            recipeIDList.Add(addMe);
                        } else
                        {
                            await context.Response.WriteAsync("Thank you for specifying a 'recipes' parameter, but, it needs to be a comma-separated list of integers. I couldn't read at least one of them.");
                            return;
                        }                        
                    }
                    AuthenticationToken tokenUser = new AuthenticationToken { ApplicationWideId = 0, ApplicationWideName = (context.Request.Query.ContainsKey("name")) ? context.Request.Query["name"].ToString() : "" };
                    ShoppingListAPI api = new ShoppingListAPI();
                    RecipeIngredient[] result = await api.GetShoppingList(recipeIDList);
                    await context.Response.WriteAsJsonAsync<RecipeIngredient[]>(result);

                } else {
                    await context.Response.WriteAsync("Nope, you need to supply a selection of Recipes using their IDs.  You can do this by adding, eg '?recipes=1,2'");
                }
                
            });
        }

        private async Task<RecipeIngredient[]> GetShoppingList(List<long> recipeIDList)
        {
            List<RecipeIngredient> buildMe = new List<RecipeIngredient>();
            Dictionary<string, Dictionary<string, Dictionary<string, Fractionable>>> ingredientInfoList = new Dictionary<string, Dictionary<string, Dictionary<string, Fractionable>>>();
            foreach (long recipeID in recipeIDList)
            {
                RecipeAPI recipeAPI = new RecipeAPI();
                using ApplicationDbContext _context = new ApplicationDbContext();                
                RecipeIngredient[] processThese = _context.RecipeIngredients.Where(ri => ri.RecipeId == recipeID).Include(recipeI => recipeI.Ingredient).ToArray();
                ingredientInfoList = AddRecipeIngredientsToList(ingredientInfoList, processThese);
            }
            //now translate into shopping list
            string ingredientName, ingredientPreparation, ingredientMeasurement;
            foreach(string lookupName in ingredientInfoList.Keys)
            {
                ingredientName = lookupName;
                Dictionary<string, Dictionary<string, Fractionable>> preparationsIntoMeasurementsIntoQuantities = ingredientInfoList[ingredientName];
                foreach(string lookupPreparation in preparationsIntoMeasurementsIntoQuantities.Keys)
                {
                    ingredientPreparation = lookupPreparation;
                    Dictionary<string, Fractionable> measurementsIntoQuantities = preparationsIntoMeasurementsIntoQuantities[lookupPreparation];
                    foreach(string lookupMeasurement in measurementsIntoQuantities.Keys)
                    {
                        ingredientMeasurement = lookupMeasurement;
                        buildMe.Add(new RecipeIngredient() { Ingredient = new Ingredient() { Name = ingredientName }, Preparation = ingredientPreparation, Measurement = ingredientMeasurement, Quantity = measurementsIntoQuantities[ingredientMeasurement] });
                    }
                }
            }
            return buildMe.ToArray();
        }

        private Dictionary<string, Dictionary<string, Dictionary<string, Fractionable>>> AddRecipeIngredientsToList(Dictionary<string, Dictionary<string, Dictionary<string, Fractionable>>> shoppingList, RecipeIngredient[] recipeIngredients)
        {
            foreach(RecipeIngredient recipeIngredient in recipeIngredients)
            {
                shoppingList = AddIngredientToList(shoppingList, recipeIngredient);
            }
            return shoppingList;
        }

        private Dictionary<string, Dictionary<string, Dictionary<string, Fractionable>>> AddIngredientToList(Dictionary<string, Dictionary<string, Dictionary<string, Fractionable>>> shoppingList, RecipeIngredient ingredientInfo)
        {
            //check if the shopping list already has that ingredient*
            //if yes, add the quantity of that ingredient to the quantity of the existing entry in the list
            //if no, add the ingredient (quantity, unit of measure, and all) into the list.
            if(shoppingList.ContainsKey(ingredientInfo.Ingredient.Name)) {
                if(shoppingList[ingredientInfo.Ingredient.Name].ContainsKey(ingredientInfo.Preparation))
                {
                    if(shoppingList[ingredientInfo.Ingredient.Name][ingredientInfo.Preparation].ContainsKey(ingredientInfo.Measurement))
                    {
                        shoppingList[ingredientInfo.Ingredient.Name][ingredientInfo.Preparation][ingredientInfo.Measurement] = shoppingList[ingredientInfo.Ingredient.Name][ingredientInfo.Preparation][ingredientInfo.Measurement] + ingredientInfo.Quantity;
                    } else
                    {
                        shoppingList[ingredientInfo.Ingredient.Name][ingredientInfo.Preparation].Add(ingredientInfo.Measurement, ingredientInfo.Quantity);
                    }
                } else
                {
                    shoppingList[ingredientInfo.Ingredient.Name].Add(ingredientInfo.Preparation, new Dictionary<string, Fractionable>());
                    shoppingList[ingredientInfo.Ingredient.Name][ingredientInfo.Preparation].Add(ingredientInfo.Measurement, ingredientInfo.Quantity);
                }
            } else
            {
                shoppingList.Add(ingredientInfo.Ingredient.Name, new Dictionary<string, Dictionary<string, Fractionable>>());
                shoppingList[ingredientInfo.Ingredient.Name].Add(ingredientInfo.Preparation, new Dictionary<string, Fractionable>());
                shoppingList[ingredientInfo.Ingredient.Name][ingredientInfo.Preparation].Add(ingredientInfo.Measurement, ingredientInfo.Quantity);
            }
            return shoppingList;
        }        
    }

}
