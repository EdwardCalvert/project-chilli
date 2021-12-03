using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorServerApp.Models;

namespace BlazorServerApp.RecipeDataProcessorService
{
    public class DietaryProcessor:IDietaryProcessor
    {
        

        public async Task<UserDefinedIngredient.Type> GetOverallType(List<UserDefinedIngredientInRecipe> userDefinedIngredients)
        {
            foreach(UserDefinedIngredientInRecipe ingredientInRecipe in userDefinedIngredients)
            {

            }
            return UserDefinedIngredient.Type.None;
        }
    }

    public interface IDietaryProcessor
    {
        public Task<UserDefinedIngredient.Type> GetOverallType(List<UserDefinedIngredientInRecipe> userDefinedIngredients);
    }
}
