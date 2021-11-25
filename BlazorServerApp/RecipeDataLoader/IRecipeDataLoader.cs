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
        public  Task InsertSearchQuery(SearchQuery search);
        public Task<List<SearchQuery>> FindWordsAPISearch(string search);
        public Task<List<Recipe>> GetHomepageRecipes(int offset);
        public  Task DeleteRecipeAndRelatedValues(uint RecipeID);
        public  Task UpdateRecipe(Recipe NewModel);
        //public  Task<Ingredient> GetIngredient(uint? IngredientID);
        public  Task IncrementViews(uint RecipeID);
        public Task<bool> DoTablesExist(string tableName);
        public Task<int> SumRecords(string tableName);
        public Task<uint> InsertRecipeAndRelatedFields(Recipe displayModel);
        public Task InsertRelatedFields(Recipe displayModel);
        public  Task<uint> InsertIngredient(UserDefinedIngredient model);
        public Task RunSql(string sql);
        public  Task<Recipe> GetRecipeAndTree(uint RecipeID);
        public  Task<string> GetIngredientName(uint ingredientID);
        public  Task<IEnumerable<UserDefinedIngredient>> FindIngredients(string text);
        public  Task<uint> EquipmentCount(Equipment equipment);
        public  Task<IEnumerable<Equipment>> FindEquipmentLike(string text);
        public Task<List<Review>> GetReviews(uint RecipeID);
        public Task SaveNewReview(Review newReview);
        public Task<uint> InsertEquipment(Equipment equipment);
        public  Task<List<Recipe>> BuildRecipeTreeFromDataModel(List<Recipe> recipes);
        public Task<int> CountNumberOfSimilarIngredients(string ingredientName);
        public  Task<List<uint>> GetSearchDatabaseTextFields(string searchText);
    }
}