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
    public class RecipeListBase : DataComponent
    {
        public string Error;

        protected override Task OnInitializedAsync()
        {
            LoadRecipies("ORDER BY PageVisits DESC LIMIT 20");
            return base.OnInitializedAsync();
        }

        //ORDER BY PageVisits DESC LIMIT 20
        public async Task<List<DisplayRecipeModel>> LoadRecipies(string extraSql)
        {
            List<RecipeDataModel> recipes;
            List<DisplayRecipeModel> UIRecipies = new List<DisplayRecipeModel>();
            recipes = await _data.LoadData<RecipeDataModel>($"SELECT  * FROM Recipe {extraSql}", _config.GetConnectionString("recipeDatabase"));
            foreach (RecipeDataModel recipe in recipes)
            {
                DisplayRecipeModel displayRecipeModel = DisplayRecipeModel.PasrseBackendToFrontend(recipe);
                List<MethodDataModel>  methods = await _data.LoadData<MethodDataModel>($"SELECT * FROM Method WHERE RecipeID={recipe.RecipeID}", _config.GetConnectionString("recipeDatabase"));
                if (methods.Count > 0)
                {
                    //Loop Parse the data model into the front end model. Should we do this here?
                    //displayRecipeModel.Method 
                }
                List<ReviewDataModel> reviews = await _data.LoadData<ReviewDataModel>($"SELECT * FROM Review WHERE RecipeID={recipe.RecipeID}",_config.GetConnectionString("recipeDatabase"));
                UIRecipies.Add(displayRecipeModel);
            }

            //UIRecipies = DisplayRecipeModel.PasrseBackendToFrontend(recipes);

            return UIRecipies;

           
        }
    }

}
