using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary;
using BlazorServerApp.Models;
using Microsoft.Extensions.Configuration;


namespace BlazorServerApp.ExtensionMethods
{
    public class SqlRunner : ComponentBase,ISqlRunner
    {
        public IEnumerable<RecipeDataModel> Recipes { get; set; }
        [Inject]
        public IDataAccess _data { get; set; }
        [Inject]
        public IConfiguration _config { get; set; }

        public async Task<List<DisplayRecipeModel>> Load(string sql)
        {
            //go to database, and get a list of the 'top' 100 recipes
            List<RecipeDataModel> recipes;
            List<DisplayRecipeModel> UIRecipies;
            recipes = await _data.LoadData<RecipeDataModel, dynamic>(sql, new { }, _config.GetConnectionString("recipeDatabase"));
            UIRecipies = DisplayRecipeModel.PasrseBackendToFrontend(recipes);

            return UIRecipies;
        }

        public async Task SaveData(string sql)
        {
            await _data.SaveData(sql, new { }, _config.GetConnectionString("recipeDatabase"));
        }

    }

    public interface ISqlRunner
    {
        public Task<List<DisplayRecipeModel>> Load(string sql);

        public  Task SaveData(string sql);


    }

}
