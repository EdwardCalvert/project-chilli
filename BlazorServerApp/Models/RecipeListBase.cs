using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary;
using BlazorServerApp.Models;
using Microsoft.Extensions.Configuration;


namespace BlazorServerApp.Models
{
    public class RecipeListBase : ComponentBase
    {
        public IEnumerable<RecipeDataModel> Recipes { get; set; }
        [Inject]
        public IDataAccess _data { get; set; }
        [Inject]
        public IConfiguration _config { get; set; }
        public string Error;

        protected override Task OnInitializedAsync()
        {
            LoadHomepageRecipies();
            return base.OnInitializedAsync();
        }

        public async Task<List<DisplayRecipeModel>> LoadHomepageRecipies()
        {
            //go to database, and get a list of the 'top' 100 recipes
            List<RecipeDataModel> recipes;
            List<DisplayRecipeModel> UIRecipies;
            
                string sql = "SELECT  * FROM Recipe ORDER BY PageVisits DESC LIMIT 20";
                recipes = await _data.LoadData<RecipeDataModel, dynamic>(sql, new { }, _config.GetConnectionString("recipeDatabase"));
                UIRecipies = DisplayRecipeModel.PasrseBackendToFrontend(recipes);

            return UIRecipies;

           
        }
    }

}
