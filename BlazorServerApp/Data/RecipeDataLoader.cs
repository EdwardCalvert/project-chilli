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
    public class RecipeDataLoader : ComponentBase
    {
        [Inject]
        private IDataAccess _data { get; set; }
        [Inject]
        private IConfiguration _config { get; set; }


        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        public string Error;

        public async Task<List<DisplayRecipeModel>> BuildRecipeTreeFromDataModel(List<DisplayRecipeModel> recipes)
        {
            foreach (DisplayRecipeModel recipe in recipes)
            {
                recipe.Method = await _data.LoadData<DisplayMethodModel, dynamic>($"SELECT * FROM Method WHERE RecipeID=@RecipeID;", new { RecipeID = recipe.RecipeID }, _config.GetConnectionString("recipeDatabase"));
                recipe.Reviews = await GetReviews(recipe.RecipeID);
                recipe.Equipment = await GetEquipment(recipe.RecipeID);
            }
            return recipes;
        }
        public async Task<List<DisplayEquipmentModel>> GetEquipment(uint RecipeID)
        {
            List<EquipmentInRecipeDataModel> models = await _data.LoadData<EquipmentInRecipeDataModel, dynamic>($"SELECT * FROM EquipmentInRecipe WHERE RecipeID=@RecipeID;", new { RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            List<DisplayEquipmentModel> equipmentDataModels = new List<DisplayEquipmentModel>();
            foreach (EquipmentInRecipeDataModel model in models)
            {
                var resutlt = await _data.LoadData<DisplayEquipmentModel, dynamic>($"SELECT * FROM Equipment WHERE EquipmentID=@EquipmentID", new { EquipmentID = model.EquipmentID }, _config.GetConnectionString("recipeDatabase"));

                equipmentDataModels.Add(resutlt[0]);
            }
            return equipmentDataModels;

        }

        public async Task<DisplayIngredientModel> GetIngredient(uint? IngredientID)
        {
            if (IngredientID != null)
            {
                List<DisplayIngredientModel> model = await _data.LoadData<DisplayIngredientModel, dynamic>($"SELECT * FROM Ingredients WHERE IngredientID =@ingredientID", new { ingredientID = IngredientID }, _config.GetConnectionString("recipeDatabase"));
                return model[0];
            }
            return null;
        }

        public async Task<List<DisplayReviewModel>> GetReviews(uint RecipeID)
        {
            List<DisplayReviewModel> reveiews = await _data.LoadData<DisplayReviewModel, dynamic>($"SELECT * FROM Review WHERE RecipeID=@RecipeID", new { RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            List<DisplayReviewModel> returnModels = new List<DisplayReviewModel>(reveiews.Count);
            foreach (DisplayReviewModel displayReview in reveiews)
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
            if (results.Count > 0)
            {
                pageVisits = results[0];
                pageVisits++;
                await _data.SaveData($"UPDATE RecipeDatabase.Recipe SET PageVisits=@PageVisits WHERE RecipeID=@RecipeID; ", new { PageVisits = pageVisits, RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
                await _data.SaveData($"UPDATE RecipeDatabase.Recipe SET LastRequested=@LastRequested WHERE RecipeID=@RecipeID; ", new
                {
                    LastRequested = RecipeDataLoader.MySQLTimeFormat(DateTime.Now),
                    RecipeID = RecipeID
                }, _config.GetConnectionString("recipeDatabase"));
            }
        }

        public async Task SaveNewReview(DisplayReviewModel review)
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

        public async Task InsertRecipeAndRelatedFields(DisplayRecipeModel displayModel)
        {
            List<uint> autoIncrementResult = await _data.LoadData<uint, dynamic>(displayModel.SqlInsertStatement() + "SELECT LAST_INSERT_ID();", displayModel.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));

            uint RecipeId = autoIncrementResult[0];
            foreach (DisplayMethodModel model in displayModel.Method)
            {
                int step;
                if (model.MethodText != null)
                {
                    model.RecipeID = RecipeId;
                    await _data.SaveData(model.SqlInsertStatement(), model.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
                }
            }

            if (displayModel.Equipment != null)
            {
                foreach (DisplayEquipmentModel model in displayModel.Equipment)
                {
                    EquipmentInRecipeDataModel model1 = new();
                    model1.EquipmentID = model.EquipmentID;
                    model1.RecipeID = RecipeId;
                    await _data.SaveData(model1.SqlInsertStatement(), model1.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
                }
            }

            if (displayModel.Ingredients != null)
            {
                foreach (DisplayIngredientInRecipeModel model in displayModel.Ingredients)
                {
                    if (model.IngredientID != default(uint))
                    {
                        
                        Console.WriteLine("attempted to submit" + model.SqlInsertStatement() + model.SqlAnonymousType((uint)model.IngredientID,RecipeId));

                        await _data.SaveData(model.SqlInsertStatement(),model.SqlAnonymousType((uint)model.IngredientID, RecipeId),_config.GetConnectionString("recipeDatabase"));
                    }
                }
            }
        }

        public async Task InsertIngredient()
        {
            //uint IngredientId, uint RecipeID,double quantity, string unit
            await _data.SaveData( "INSERT INTO IngredientsInRecipe VALUES(@ingredientID,@recipeID,@Quantity,@Unit);",new { ingredientID = 13755, recipeID = 29,Quantity = 1, Unit = "Cheese"}, _config.GetConnectionString("recipeDatabse"));
        }

        public async Task ProcessEquipmentCsvAndSaveToDB(string csvData)
        {
            string[] lines = csvData.Split(Environment.NewLine);
            foreach (string line in lines)
            {

                string[] cols = line.Split(',');
                EquipmentDataModel model = new EquipmentDataModel();

                if (cols.Length > 0)
                {
                    model.EquipmentName = cols[0];
                    if (EquipmentDataModel.Types.Contains(cols[1]))
                    {
                        model.TypeOf = cols[1];
                        //Check to see if a similar record exists. 
                        List<EquipmentDataModel> equipment = await _data.LoadData<EquipmentDataModel, dynamic>("SELECT * FROM Equipment WHRE EquipmentName LIKE @equipmentName", new { equipmentName = model.EquipmentName }, _config.GetConnectionString("recipeDatabase"));
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

        public async Task ProcessIngredientCsvAndSaveToDB(string csvData)
        {
            string[] lines = csvData.Split(Environment.NewLine);
            foreach (string line in lines)
            {
                await Task.Run(() => InsertIngredientIntoDB(line));

            }
        }

        public async Task InsertIngredientIntoDB(string line)
        {
            string[] cols = line.Split(',');
            if (cols.Length == 20)
            {
                if (Regex.IsMatch(cols[0], "[0-9]{2}-[0-9]{3,4}"))
                {
                    DisplayIngredientModel model = new DisplayIngredientModel();
                    model.FoodCode = cols[0];
                    //Check to see if a similar record exists. 
                    List<DisplayIngredientModel> existingIngredients = await _data.LoadData<DisplayIngredientModel, dynamic>("SELECT * FROM Ingredients WHERE FoodCode=@FoodCode;", new { FoodCode = model.FoodCode }, _config.GetConnectionString("recipeDatabase"));

                    if (existingIngredients.Count == 0)
                    {
                        model.IngredientName = cols[1].Replace('#', ',');
                        int rowindex = 2;
                        foreach (string property in model)
                        {
                            if (double.TryParse(cols[rowindex], out double number)) ;
                            {
                                model.GetType().GetProperty(property).SetValue(model, number, null);
                            }
                            rowindex++;
                        }
                        model.AlternateName = model.IngredientName.ReverseCSVValues().SentenceCase();
                        await _data.SaveData(model.SqlInsertStatement(), model.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
                    }
                }
            }
        }

        public async Task<IEnumerable<DisplayEquipmentModel>> FindEquipmentLike(string text)
        {
            return await _data.LoadData<DisplayEquipmentModel, dynamic>("SELECT *  FROM Equipment WHERE EquipmentName LIKE Concat('%',@Text,'%') LIMIT 20;", new { Text = text }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<IEnumerable<DisplayIngredientModel>> FindIngredients(string text)
        {
            text = $"%{text}%".Replace(" ", "%");
            List<DisplayIngredientModel> result1= await _data.LoadData<DisplayIngredientModel, dynamic>("SELECT *  FROM Ingredients WHERE AlternateName LIKE @Text LIMIT 30;", new { Text = text }, _config.GetConnectionString("recipeDatabase"));
            List<DisplayIngredientModel> result2 = await _data.LoadData<DisplayIngredientModel, dynamic>("SELECT *  FROM Ingredients WHERE IngredientName LIKE @Text LIMIT 30;", new { Text = text }, _config.GetConnectionString("recipeDatabase"));
            result1.AddRange(result2);
            return result1;
        }



        public async Task<List<DisplayRecipeModel>> GetHomepageRecipes()
        {
            List<DisplayRecipeModel> datas = await _data.LoadData<DisplayRecipeModel, dynamic>("SELECT * FROM Recipe ORDER BY PageVisits DESC LIMIT 20", new { }, _config.GetConnectionString("recipeDatabase"));
            foreach (DisplayRecipeModel model in datas)
            {
                model.DisplayNutritionModel = new DisplayNutritionModel(model.Kcal, model.Fat, model.Saturates, model.Sugar, model.Fibre, model.Carbohydrates, model.Salt, DisplayRecipeModel.RecomendedIntake);
            }
            return await BuildRecipeTreeFromDataModel(datas);
        }

        public async Task<List<DisplayRecipeModel>> GetRecipe(uint RecipeID)
        {
            List<DisplayRecipeModel> result = await _data.LoadData<DisplayRecipeModel, dynamic>("SELECT * FROM Recipe WHERE RecipeID = @recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            foreach (DisplayRecipeModel model in result)
            {
                model.DisplayNutritionModel = new DisplayNutritionModel(model.Kcal, model.Fat, model.Saturates, model.Sugar, model.Fibre, model.Carbohydrates, model.Salt, DisplayRecipeModel.RecomendedIntake);
            }

            return await BuildRecipeTreeFromDataModel(result);
        }

        public async Task RunSql(string sql)
        {
            await _data.SaveData(sql, new { }, _config.GetConnectionString("recipeDatabase"));
        }
    }

}
