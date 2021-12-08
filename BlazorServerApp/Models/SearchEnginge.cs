using BlazorServerApp.TextProcessor;
using BlazorServerApp.WordsAPI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class SearchEnginge
    {
        public enum SearchFields //Things that make sense to be looked for in the database
        {
            RecipeName,
            Equipment,
            Servings,
            CookingTime,
            PreperationTime,
            Reviews,
            PageVisits,
        };

        public enum SortBy //Things that searching for doesn't make sense, but limiting them at the end does
        {
            MealType,
            Difficulty,
            Reviews,
            RecipeName,
            Servings,
            CookingTime,
            PreperationTime,
            PageVisits,
            Default,
        }

        public enum Order
        {
            Ascending,
            Descending,
        }

        public static async Task<List<Recipe>> SearchForRecipes(IRecipeDataLoader dataLoader, string searchTerm, IWordsAPIService wordsAPIService, int offset, UserDefinedIngredient.Type type)
        {
            List<Recipe> searchResults = new List<Recipe>();
            List<uint> recipeIDs = new();
            searchTerm = searchTerm.ToLower();
            //TextProcessor.TextProcessor.
            //UserDefinedIngredient.Type  type=UserDefinedIngredient.GetTypeEnum(searchTerm);
            TextProcessor.TextProcessor textProcessor = new TextProcessor.TextProcessor(new NounExtractor(), wordsAPIService, dataLoader);

            if (searchTerm.Contains("vegan"))
            {
                type = UserDefinedIngredient.Veganism;
            }
            else if (searchTerm.Contains("vegetarian"))
            {
                type = UserDefinedIngredient.Vegetarianism;
            }

            if (type != UserDefinedIngredient.Type.None)
            {
                recipeIDs.AddRange(await dataLoader.GetSearchDatabaseTextFields(searchTerm, offset, (ushort)~(ushort)type)); //
            }
            else
            {
                List<UserDefinedIngredientInRecipe> ingredientsInRecipes = await textProcessor.GetIngredientsWithUnits(searchTerm, false);

                if (ingredientsInRecipes.Count > 0)
                {
                    foreach (UserDefinedIngredientInRecipe ingredient in ingredientsInRecipes)
                    {
                        recipeIDs.Add((uint)ingredient.IngredientID);
                    }
                }
                recipeIDs.AddRange(await dataLoader.GetSearchDatabaseTextFields(searchTerm, offset));
            }

            foreach (uint recipeID in recipeIDs)
            {
                Recipe r = await dataLoader.GetRecipeAndTree(recipeID);
                searchResults.Add(r);
            }
            return searchResults;
        }
    }
}