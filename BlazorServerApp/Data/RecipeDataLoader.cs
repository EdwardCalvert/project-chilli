using DataLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlazorServerApp.Extensions;

namespace BlazorServerApp.Models
{
    public interface IRecipeDataLoader
    {
        public  Task<List<Recipe>> GetHomepageRecipes();
    }

    public class RecipeDataLoader : ComponentBase, IRecipeDataLoader
    {
        //[Inject]
        private IDataAccess _data { get; set; }
        //[Inject]
        private IConfiguration _config { get; set; }


       public RecipeDataLoader(IDataAccess data, IConfiguration config)
        {
            _data = data;
            _config = config;
        }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }


        public async Task DeleteMethodModels(List<Method> displayMethodModels)
        {
            foreach (Method displayMethodModel in displayMethodModels)
            {
               await _data.SaveData(displayMethodModel.SqlDeleteStatement(), displayMethodModel.SqlDeleteAnonymousType(), _config.GetConnectionString("recipeDatabase"));
            }

        }

        public async Task DeleteEquipmentModels(uint RecipeID)
        {
            await _data.SaveData("DELETE FROM EquipmentInRecipe WHERE RecipeID = @recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task DeleteIngredientInRecipeModel(uint RecipeID)
        {
            await _data.SaveData("DELETE FROM IngredientsInRecipe WHERE RecipeID = @recipeID",new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }


        public async Task UpdateRecipe(Recipe NewModel)
        {
            List<Recipe> models = await GetRecipe(NewModel.RecipeID);
            Recipe staleModel = models[0];
            await DeleteMethodModels(staleModel.Method);
            await DeleteEquipmentModels(NewModel.RecipeID);
            await DeleteIngredientInRecipeModel(NewModel.RecipeID);
            await _data.SaveData(NewModel.SqlUpdateStatement(), NewModel.SqlAnonymousType(NewModel.RecipeID),_config.GetConnectionString("recipeDatabase"));
            await InsertRelatedFields(NewModel);
        }

        public async Task<List<Recipe>> BuildRecipeTreeFromDataModel(List<Recipe> recipes)
        {
            
            foreach (Recipe recipe in recipes)
            {
                if (ValidRecipeID.Validate(recipe.RecipeID))
                {
                    recipe.DisplayNutritionModel = new DisplayNutritionModel(recipe.Kcal, recipe.Fat, recipe.Saturates, recipe.Sugar, recipe.Fibre, recipe.Carbohydrates, recipe.Salt, Recipe.RecomendedIntake);
                    recipe.Method = await GetMethod(recipe.RecipeID);
                    recipe.Reviews = await GetReviews(recipe.RecipeID);
                    recipe.Equipment = await GetEquipment(recipe.RecipeID);
                    recipe.Ingredients = await GetIngredientsInRecipe(recipe.RecipeID);
                }
                else
                {
                    throw new Exception("The provided recipe ID doesn't seem valid");
                }
            }
            return recipes;
        }
        public async Task<List<Method>> GetMethod(uint RecipeID)
        {
            return await _data.LoadData<Method, dynamic>($"SELECT * FROM Method WHERE RecipeID=@RecipeID;", new { RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<List<Equipment>> GetEquipment(uint RecipeID)
        {
            List<EquipmentInRecipe> models = await _data.LoadData<EquipmentInRecipe, dynamic>($"SELECT * FROM EquipmentInRecipe WHERE RecipeID=@RecipeID;", new { RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            List<Equipment> equipmentDataModels = new List<Equipment>();
            foreach (EquipmentInRecipe model in models)
            {
                var resutlt = await _data.LoadData<Equipment, dynamic>($"SELECT * FROM Equipment WHERE EquipmentID=@EquipmentID", new { EquipmentID = model.EquipmentID }, _config.GetConnectionString("recipeDatabase"));

                equipmentDataModels.Add(resutlt[0]);
            }
            return equipmentDataModels;

        }

        public async Task<List<IngredientInRecipe>> GetIngredientsInRecipe(uint RecipeID)
        {
             return await _data.LoadData<IngredientInRecipe, dynamic>($"SELECT * FROM IngredientsInRecipe WHERE RecipeID =@recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<List<Ingredient>> GetIngredientModel(uint IngredientID)
        {
            return  await _data.LoadData<Ingredient, dynamic>($"SELECT * FROM Ingredients WHERE IngredientID =@ingredientID", new { ingredientID = IngredientID }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<Ingredient> GetIngredient(uint? IngredientID)
        {
            if (IngredientID != null)
            {
                List<Ingredient> model = await GetIngredientModel((uint)IngredientID);
                return model[0];
            }
            return null;
        }

        public async Task<List<Review>> GetReviews(uint RecipeID)
        {
            List<Review> reveiews = await _data.LoadData<Review, dynamic>($"SELECT * FROM Review WHERE RecipeID=@RecipeID", new { RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            List<Review> returnModels = new List<Review>(reveiews.Count);
            foreach (Review displayReview in reveiews)
            {
                displayReview.Star = Star.CreateStar(displayReview.StarCount);
                returnModels.Add(displayReview);
            }
            return returnModels;
        }

        public static string MySQLTimeFormat(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public async Task IncrementViews(uint RecipeID)
        {
            uint pageVisits = 0;
            List<uint> results = await _data.LoadData<uint, dynamic>($"SELECT PageVisits FROM Recipe WHERE RecipeID=@RecipeID", new { RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            if (results.Count == 0)
            {
                pageVisits = results[0];
                pageVisits++;
                await _data.SaveData($"UPDATE RecipeDatabase.Recipe SET PageVisits=@PageVisits,  LastRequested=@LastRequested  WHERE RecipeID=@RecipeID; ", new { PageVisits = pageVisits, LastRequested = MySQLTimeFormat(DateTime.Now), RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            }
        }

        public async Task SaveNewReview(Review review)
        {
            await _data.SaveData(review.SQLInsertStatement(), review.SQLAnonymousType(), _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<bool> DoTablesExist(string tableName)
        {
            List<string> returnData = new List<string>();
            returnData = await _data.LoadData<string, dynamic>($"SHOW TABLES LIKE @TableName;", new { TableName = tableName }, _config.GetConnectionString("recipeDatabase"));
            return returnData.Count != 0;
        }

        public async Task<int> SumRecords(string tableName)
        {
            List<int> sum = await _data.LoadData<int, dynamic>($"SELECT count( * ) as  total_record FROM {tableName};", new { tableName = tableName }, _config.GetConnectionString("recipeDatabase"));
            if (sum.Count > 0)
            {
                return sum[0];
            }
            return 0;
        }

        public async Task InsertRecipeAndRelatedFields(Recipe displayModel)
        {
            List<uint> autoIncrementResult = await _data.LoadData<uint, dynamic>(displayModel.SqlInsertStatement() + "SELECT LAST_INSERT_ID();", displayModel.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
            uint RecipeId = autoIncrementResult[0];
            displayModel.RecipeID = RecipeId;
            await InsertRelatedFields(displayModel);
        }

        public async Task ProcessEquipmentCsvAndSaveToDB(string csvData)
        {
            string[] lines = csvData.Split(Environment.NewLine);
            foreach (string line in lines)
            {

                string[] cols = line.Split(',');
                Equipment model = new Equipment();

                if (cols.Length > 0)
                {
                    model.EquipmentName = cols[0];
                    if (Equipment.Types.Contains(cols[1]))
                    {
                        model.TypeOf = cols[1];
                        //Check to see if a similar record exists. 
                        List<Equipment> equipment = await _data.LoadData<Equipment, dynamic>("SELECT * FROM Equipment WHRE EquipmentName LIKE @equipmentName", new { equipmentName = model.EquipmentName }, _config.GetConnectionString("recipeDatabase"));
                        if (equipment.Count == 0)
                        {
                            await _data.SaveData(model.SqlInsertStatement(), model.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
                        }
                    }
                    else
                    {
                        throw new Exception("The provided file is not considered valid (The 'TypeOf' field is incorrect). Please use the docs to understand the correct file type. ");
                    }
                }

            }

        }

        public async Task InsertRelatedFields(Recipe displayModel)
        {
            
            foreach (Method model in displayModel.Method)
            {
                if (model.MethodText != null)
                {
                    model.RecipeID = displayModel.RecipeID;
                    await _data.SaveData(model.SqlInsertStatement(), model.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
                }
            }

            if (displayModel.Equipment != null)
            {
                foreach (Equipment model in displayModel.Equipment)
                {
                    EquipmentInRecipe model1 = new();
                    model1.EquipmentID = model.EquipmentID;
                    model1.RecipeID = displayModel.RecipeID;
                    await _data.SaveData(model1.SqlInsertStatement(), model1.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
                }
            }

            if (displayModel.Ingredients != null)
            {
                foreach (IngredientInRecipe model in displayModel.Ingredients)
                {
                    if (model.IngredientID != default(uint))
                    {

                        Console.WriteLine("attempted to submit" + model.SqlInsertStatement() + model.SqlAnonymousType((uint)model.IngredientID, displayModel.RecipeID));

                        await _data.SaveData(model.SqlInsertStatement(), model.SqlAnonymousType((uint)model.IngredientID, displayModel.RecipeID), _config.GetConnectionString("recipeDatabase"));
                    }
                }
            }
        }
        public async Task<List<Ingredient>> FindExistingIngredients(string foodName)
        {
            return await _data.LoadData<Ingredient, dynamic>("SELECT * FROM Ingredients WHERE IngredientName=@FoodCode;", new { FoodCode = foodName }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task InsertIngredient(Ingredient model)
        {
            await _data.SaveData(model.SqlInsertStatement(), model.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
        }

        public async Task InsertIngredientIntoDB(string line, bool runDeduplication)
        {
            string[] cols = line.Split(',');
            if (cols.Length == 20)
            {

                Ingredient model = new Ingredient();
                model.FoodGroup = cols[1].Replace('#', ','); ;
            model.IngredientName = cols[0];
                //Check to see if a similar record exists. 
                int existingIngredients = 0;
                if (runDeduplication)
                {
                    Console.WriteLine("Running deup");
                    List<Ingredient> result = await FindExistingIngredients(model.IngredientName);
                    existingIngredients = result.Count;

                }
                if (existingIngredients ==0)
                {
                    int rowindex = 2;
                    foreach (string property in model)
                    {
#pragma warning disable CS0642 // Possible mistaken empty statement
                        if (double.TryParse(cols[rowindex], out double number)) ;
#pragma warning restore CS0642 // Possible mistaken empty statement
                        {
                            model.GetType().GetProperty(property).SetValue(model, number, null);
                        }
                        rowindex++;
                    }
                    model.AlternateName = model.IngredientName.ReverseCSVValues().SentenceCase();
                    await InsertIngredient(model);
                }

            }
        }

        public async Task<IEnumerable<Equipment>> FindEquipmentLike(string text)
        {
            return await _data.LoadData<Equipment, dynamic>("SELECT *  FROM Equipment WHERE EquipmentName LIKE Concat('%',@Text,'%') LIMIT 20;", new { Text = text }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<IEnumerable<Ingredient>> FindIngredients(string text)
        {
            text = $"%{text}%".Replace(" ", "%");
            return await _data.LoadData<Ingredient, dynamic>("SELECT *  FROM Ingredients WHERE IngredientName LIKE @Text OR AlternateName LIKE @Text LIMIT 100;", new { Text = text }, _config.GetConnectionString("recipeDatabase")); ;
        }


        public async Task<string> GetIngredientName(uint ingredientID)
        {
           List<string> results =  await _data.LoadData<string, dynamic>("SELECT IngredientName From Ingredients WHERE IngredientID = @ingredientID", new { ingredientID = ingredientID }, _config.GetConnectionString("recipeDatabase"));
            return results[0];
        }

        public async Task<List<Recipe>> GetHomepageRecipes()
        {
            List<Recipe> datas = await _data.LoadData<Recipe, dynamic>("SELECT * FROM Recipe ORDER BY PageVisits DESC LIMIT 20", new { }, _config.GetConnectionString("recipeDatabase"));
            return await BuildRecipeTreeFromDataModel(datas);
        }


        public async Task<List<Recipe>> GetRecipe(uint RecipeID)
        {
            List<Recipe> result = await _data.LoadData<Recipe, dynamic>("SELECT * FROM Recipe WHERE RecipeID = @recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            return await BuildRecipeTreeFromDataModel(result);
        }

        public async Task RunSql(string sql)
        {
            await _data.SaveData(sql, new { }, _config.GetConnectionString("recipeDatabase"));
        }

      
    }

}
