using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorServerApp.HelperMethods;
using BlazorServerApp.Models;

namespace BlazorServerApp.Models
{
    public class SearchEnginge
    {
        public static readonly List<string> KEYWORDS = new List<string>()
        {
            "equipment",
            "time",
            "ingredient"
        };

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

        public static async Task<List<Recipe>> SearchForRecipes(IRecipeDataLoader dataLoader, string searchTerm,SortBy sortBy,Order order)
        {
            List<Recipe> searchResults = new List<Recipe>();
            searchTerm = searchTerm.ToLower();
            //foreach(string keywords in SearchEnginge.KEYWORDS) {
            //    if (searchTerm.Contains(keywords))
            //    {
            //        //Find the approprite search method. And return it?
            //    }
            //}

            List<uint> recipeIDs = await dataLoader.GetSearchDatabaseTextFields(searchTerm);
            foreach (uint recipeID in recipeIDs)
            {
                Recipe r = await dataLoader.GetRecipeAndTree(recipeID);
                searchResults.Add(r);
            }


            //Sort the results, according to how the user wishes
            if (sortBy == SortBy.Default)
            {
                return searchResults;
            }
            else if(sortBy == SortBy.Reviews)
            {
                //Do something else (since reviews need to use the.average function!)
                return searchResults;
            }
            else
            {
                return  MergeSort.RecipeMergeSort(searchResults, sortBy,order);
            }
        }

       
    }
}
