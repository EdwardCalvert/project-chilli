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
        public  Task<bool> ContainsStopWord(string searchTerm);
        public  Task InsertSearchQuery(SearchQuery search);
        public Task<List<SearchQuery>> FindWordsAPISearch(string search);
        public Task<List<Recipe>> GetHomepageRecipes(int offset);
        public  Task DeleteRecipeAndRelatedValues(uint RecipeID);
        public  Task UpdateRecipe(Recipe NewModel);
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
        public  Task<List<uint>> GetSearchDatabaseTextFields(string searchText,int offset);
        public Task<List<uint>> GetSearchDatabaseTextFields(string searchText, int offset, ushort invertedTypeOfBitPattern);
        public Task<List<uint>> GetRecipeIDFromIngredientID(int quantity, string unit, uint userDefinedIngredientID);
        public Task<List<UserDefinedIngredientInRecipe>> GetIngredientsInRecipe(uint RecipeID);

        public  Task SaveUser(User user);
        public  Task<User> GetUserFromDatabase(string userName);
        public  Task<List<Recipe>> BulkImportRecipes(int offset);
        public Task<List<User>> GetAllUsers();
        public  Task DeleteUser(string username);
        public Task UpdatePassword(User user);

    }
}