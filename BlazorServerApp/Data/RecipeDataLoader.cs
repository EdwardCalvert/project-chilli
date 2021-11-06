using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary;
using BlazorServerApp.Models;
using Microsoft.Extensions.Configuration;
using BlazorServerApp.Data;

namespace BlazorServerApp.Models
{
    public class RecipeDataLoader : ComponentBase
    {
        [Inject]
        public IDataAccess _data { get; set; }
        [Inject]
        public IConfiguration _config { get; set; }


        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        public string Error;

        public async Task<List<DisplayRecipeModel>> LoadRecipies(string extraSql)
        {
            List<RecipeDataModel> recipes = new List<RecipeDataModel>();
            List<DisplayRecipeModel> UIRecipies = new List<DisplayRecipeModel>();
            recipes = await _data.LoadData<RecipeDataModel>($"SELECT * FROM Recipe {extraSql}", _config.GetConnectionString("recipeDatabase"));
            foreach (RecipeDataModel recipe in recipes)
            {
                DisplayRecipeModel displayRecipeModel = DisplayRecipeModel.PasrseBackendToFrontend(recipe);
                List<MethodDataModel>  methods = await _data.LoadData<MethodDataModel>($"SELECT * FROM Method WHERE RecipeID={recipe.RecipeID}", _config.GetConnectionString("recipeDatabase"));
                if (methods != null &&methods.Count > 0)
                {
                    displayRecipeModel.Method = ModelParser.ParseMethodDataModelToDisplayMethodModel(methods);
                }
                List<ReviewDataModel> reviews = await _data.LoadData<ReviewDataModel>($"SELECT * FROM Review WHERE RecipeID={recipe.RecipeID}",_config.GetConnectionString("recipeDatabase"));
                if (reviews != null &&reviews.Count > 0)
                {
                    displayRecipeModel.Reviews=ModelParser.ParseReviewDataModelToDisplayReviewModel(reviews);
                            
                }
                UIRecipies.Add(displayRecipeModel);
            }
            return UIRecipies;
        }

        public async Task<List<ReviewDataModel>> GetReviews(int RecipeID)
        {
            return await _data.LoadData<ReviewDataModel>($"SELECT * FROM Review WHERE RecipeID={RecipeID}",_config.GetConnectionString("recipeDatabase"));
        }

        public static string MySQLTimeFormat(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public async Task IncrementViews(int RecipeID)
        {
            int pageVisits = 0;
            List<int> results = await _data.LoadData<int>($"SELECT PageVisits FROM Recipe WHERE RecipeID={RecipeID}", _config.GetConnectionString("recipeDatabase"));
            pageVisits = results[0];
            pageVisits++;
            await _data.SaveData($"UPDATE RecipeDatabase.Recipe SET PageVisits={pageVisits} WHERE RecipeID={RecipeID}; ", _config.GetConnectionString("recipeDatabase"));
            await _data.SaveData($"UPDATE RecipeDatabase.Recipe SET LastRequested='{RecipeDataLoader.MySQLTimeFormat(DateTime.Now)}' WHERE RecipeID={RecipeID}; ", _config.GetConnectionString("recipeDatabase"));
        }

        public async Task SaveNewReview(DisplayReviewModel displayReviewModel)
        {
            await _data.SaveData(ModelParser.ParseDisplayReviewModelToDisplayDataModel(displayReviewModel).SQLInsertStatement(),_config.GetConnectionString("recipeDatabase"));
        }

    }

}
