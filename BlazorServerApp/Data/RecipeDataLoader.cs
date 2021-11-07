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
        private IDataAccess _data { get; set; }
        [Inject]
        private IConfiguration _config { get; set; }


        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        public string Error;

        public async Task<List<DisplayRecipeModel>> BuildRecipeTreeFromDataModel(List<RecipeDataModel> recipes)
        {
            List<DisplayRecipeModel> UIRecipies = new List<DisplayRecipeModel>();
            foreach (RecipeDataModel recipe in recipes)
            {
                DisplayRecipeModel displayRecipeModel = ModelParser.ParseOnlyRecipeDataModelIntoDisplayRecipeModel(recipe);
                List<MethodDataModel>  methods = await _data.LoadData<MethodDataModel, dynamic>($"SELECT * FROM Method WHERE RecipeID=@RecipeID;",new { RecipeID = recipe.RecipeID }, _config.GetConnectionString("recipeDatabase"));
                if (methods != null &&methods.Count > 0)
                {
                    displayRecipeModel.Method = ModelParser.ParseMethodDataModelToDisplayMethodModel(methods);
                }
                List<ReviewDataModel> reviews = await _data.LoadData<ReviewDataModel, dynamic>($"SELECT * FROM Review WHERE RecipeID=@RecipeID", new { RecipeID = recipe.RecipeID }, _config.GetConnectionString("recipeDatabase"));
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
            return await _data.LoadData<ReviewDataModel,dynamic>($"SELECT * FROM Review WHERE RecipeID=@RecipeID",new { RecipeID = RecipeID },_config.GetConnectionString("recipeDatabase"));
        }

        public static string MySQLTimeFormat(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public async Task IncrementViews(int RecipeID)
        {
            int pageVisits = 0;
            List<int> results = await _data.LoadData<int, dynamic>($"SELECT PageVisits FROM Recipe WHERE RecipeID=@RecipeID", new { RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            pageVisits = results[0];
            pageVisits++;
            await _data.SaveData($"UPDATE RecipeDatabase.Recipe SET PageVisits=@PageVisits WHERE RecipeID=@RecipeID; ",new {PageVisits = pageVisits, RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            await _data.SaveData($"UPDATE RecipeDatabase.Recipe SET LastRequested=@LastRequested WHERE RecipeID=@RecipeID; ",new {LastRequested =  RecipeDataLoader.MySQLTimeFormat(DateTime.Now), RecipeID = RecipeID
        }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task SaveNewReview(DisplayReviewModel displayReviewModel)
        {
            ReviewDataModel review = ModelParser.ParseDisplayReviewModelToDisplayDataModel(displayReviewModel);
            await _data.SaveData(review.SQLInsertStatement(),review.SQLAnonymousType(),_config.GetConnectionString("recipeDatabase"));
        }

        public async Task<bool> DoTablesExist(string tableName)
        {
            List<string> returnData = new List<string>();
            returnData = await _data.LoadData<string,dynamic>($"SHOW TABLES LIKE @TableName;",new {TableName = tableName  }, _config.GetConnectionString("recipeDatabase"));
            return returnData.Count != 0;
        }

        public async Task<int> SumRecords(string tableName)
        {
            List<int> sum = await _data.LoadData<int,dynamic>($"SELECT count( * ) as  total_record FROM {tableName};", new { tableName = tableName}, _config.GetConnectionString("recipeDatabase"));
            if (sum.Count > 0)
            {
                return sum[0];
            }
            return 0;
        }

        public async Task InsertRecipeAndRelatedFields(DisplayRecipeModel displayRecipeModel )
        {

            

            RecipeDataModel dataModel = ModelParser.ParseOnlyDisplayRecipeModelIntoRecipeDataModel(displayRecipeModel);


            List<uint> autoIncrementResult = await _data.LoadData<uint,dynamic>(dataModel.SqlInsertStatement() + "SELECT LAST_INSERT_ID();", dataModel.SqlAnonymousType() ,_config.GetConnectionString("recipeDatabase"));

            uint RecipeId = autoIncrementResult[0];
            dataModel.RecipeID = RecipeId;


            List<MethodDataModel> methodData = ModelParser.ParseDisplayMethodModelToMethodDataModel(displayRecipeModel.Method, dataModel.RecipeID);
            foreach (MethodDataModel model in methodData)
            {
                if (model.MethodText != null)
                {
                    await _data.SaveData(model.SqlInsertStatement(), model.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
                }
            }

            
            List<EquipmentDataModel> equipmentData = ModelParser.ParseDisplayEquipmentModelToEquipmentDataModel(displayRecipeModel.Equipment);
            foreach (EquipmentDataModel model in equipmentData)
            {

                EquipmentInRecipeDataModel model1 = new();
                model1.EquipmentID = model.EquipmentID;
                model1.RecipeID = dataModel.RecipeID;
                await _data.SaveData(model1.SqlInsertStatement(), model1.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
            }
        }

        public async Task ProcessCSVSaveToDB(string csvData)
        {
             string[] lines = csvData.Split(Environment.NewLine);
            foreach(string line in lines)
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
                        List<EquipmentDataModel> equipment = await _data.LoadData<EquipmentDataModel, dynamic>("SELECT * FROM Equipment WHRE EquipmentName LIKE @equipmentName", new { equipmentName = model.EquipmentName},_config.GetConnectionString("recipeDatabase"));
                        if (equipment.Count ==0)
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

    }

}
