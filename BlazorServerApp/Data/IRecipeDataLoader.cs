using BlazorServerApp.Extensions;
using DataLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BlazorServerApp.Models
{
    public interface IRecipeDataLoader
    {
        public Task<List<Recipe>> GetHomepageRecipes();
        public  Task DeleteRecipeAndRelatedValues(uint RecipeID);
        public  Task UpdateRecipe(Recipe NewModel);
        //public  Task<Ingredient> GetIngredient(uint? IngredientID);
        public  Task IncrementViews(uint RecipeID);
        public Task<bool> DoTablesExist(string tableName);
        public Task<int> SumRecords(string tableName);
        public Task InsertRecipeAndRelatedFields(Recipe displayModel);
        public Task InsertRelatedFields(Recipe displayModel);
        public  Task<uint> InsertIngredient(UserDefinedIngredient model);
        public Task RunSql(string sql);
        public  Task<List<Recipe>> GetRecipe(uint RecipeID);
        public  Task<string> GetIngredientName(uint ingredientID);
        public  Task<IEnumerable<UserDefinedIngredient>> FindIngredients(string text);
        public  Task<uint> EquipmentCount(Equipment equipment);
        public  Task<IEnumerable<Equipment>> FindEquipmentLike(string text);
        public Task<List<Review>> GetReviews(uint RecipeID);
        public Task SaveNewReview(Review newReview);
        public Task InsertEquipment(Equipment equipment);
        public  Task<List<Recipe>> BuildRecipeTreeFromDataModel(List<Recipe> recipes);
    }
}