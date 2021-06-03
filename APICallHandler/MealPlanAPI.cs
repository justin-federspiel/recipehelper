using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace APICallHandler
{
    public class MealPlanAPI
    {
        public static void MapAPIEndpoints(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/mealplan/craft", async (context) =>
            {
                int numberOfMeals = 0, numberOfRecipesPerMeal = 0;
                if(!context.Request.Query.ContainsKey("number-meals") || !int.TryParse(context.Request.Query["number-meals"].ToString(), out numberOfMeals))
                {
                    await context.Response.WriteAsync("In order to make a meal plan, we need a number of meals to include in the meal plan.  Please use an integer number of meals.");
                    return;
                }
                if (!context.Request.Query.ContainsKey("recipes-per-meal") || !int.TryParse(context.Request.Query["recipes-per-meal"].ToString(), out numberOfRecipesPerMeal))
                {
                    await context.Response.WriteAsync("In order to make a meal plan, we need a number of recipes per meal to include in the meal plan.  Please use an integer number of recipes.");
                    return;
                }
                MealPlan result = new MealPlan() { Meals = new List<Meal>() };
                for(int mealCounter = 0; mealCounter < numberOfMeals; mealCounter++)
                {
                    Meal newMeal = new Meal() { Recipes = new List<Recipe>() };
                    for (int recipeCounter = 0; recipeCounter < numberOfRecipesPerMeal; recipeCounter++)
                    {
                        newMeal.Recipes.Add(new Recipe());
                    }
                    result.Meals.Add(newMeal);
                } 
                await context.Response.WriteAsJsonAsync<MealPlan>(result);
            });
        }
    }
}
