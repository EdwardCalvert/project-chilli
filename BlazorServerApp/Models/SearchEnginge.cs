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

        public static async Task<List<Recipe>> SearchForRecipes(IRecipeDataLoader dataLoader, string searchTerm, IWordsAPIService wordsAPIService, int offset, Ingredient.Type type)
        {
            List<Recipe> searchResults = new List<Recipe>();
            List<uint> recipeIDs = new();
           

            if (searchTerm == "all")
            {
                recipeIDs.AddRange(await dataLoader.FullSearch( offset, (ushort)~(ushort)type));
            }
            else if (type != Ingredient.Type.None)
            {
                recipeIDs.AddRange(await dataLoader.GetSearchDatabaseTextFields(searchTerm, offset, (ushort)~(ushort)type)); //
            }
            else
            {
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