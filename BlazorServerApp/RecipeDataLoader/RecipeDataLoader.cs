using DataLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace BlazorServerApp.Models
{
    public class RecipeDataLoader : ComponentBase, IRecipeDataLoader
    {
        private IDataAccess _data { get; set; }
        private IConfiguration _config { get; set; }

        public RecipeDataLoader(IDataAccess data, IConfiguration config)
        {
            try
            {
                _data = data;
                _config = config;
                List<string> results = _data.LoadData<string, dynamic>("select schema_name from information_schema.schemata where schema_name = 'RecipeDatabase';", new { }, _config.GetConnectionString("wholeDatabase")).Result;
                if (results.Count == 0)
                {
                    Console.WriteLine("Need to create database");
                    Task.Run(()=>CreateDatabase());
                }
                else
                {
                    Console.WriteLine("No need to create database");
                }
            }
            catch
            {
                Console.WriteLine("An error occured while attempting to connect to the database. You may need to start the SQL server, or change the connection string.");
            }
             
        }

        private async Task CreateDatabase()
        {
            string SQLCreateCommand;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "RecipeDatabase.sql");
            if (File.Exists(path)) {
                using (StreamReader stream = new StreamReader(path))
                {
                    SQLCreateCommand = stream.ReadToEnd();
                }
                await _data.SaveData(SQLCreateCommand, new { }, _config.GetConnectionString("wholeDatabase"));
                Console.WriteLine("Database created");
            }
            else
            {
                Console.WriteLine("Unable to create database: 'RecipeDatabase.sql' no such file exists");
            }

        }

        public async Task<bool> ContainsStopWord(string searchTerm)
        {
            List<int> resutl = await _data.LoadData<int, dynamic>("SELECT COUNT(*) FROM StopWords WHERE VALUE = @searchTem;", new { searchTem = searchTerm }, _config.GetConnectionString("recipeDatabase"));
            return resutl[0] == 1;
        }

        public async Task InsertSearchQuery(SearchQuery search)
        {
            await _data.SaveData(search.SqlInsertStatement(), search.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<List<SearchQuery>> FindWordsAPISearch(string search)
        {
            return await _data.LoadData<SearchQuery, dynamic>("SELECT * FROM SearchQuery WHERE SearchTerm = @searchTerm", new { searchTerm = search }, _config.GetConnectionString("recipeDatabase"));
        }

        private async Task DeleteMethod(uint RecipeID)
        {
            await _data.SaveData("DELETE FROM Method WHERE RecipeID = @recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        private async Task DeleteEquipment(uint RecipeID)
        {
            await _data.SaveData("DELETE FROM EquipmentInRecipe WHERE RecipeID = @recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<UserDefinedIngredient> GetUserDefinedIngredient(UserDefinedIngredientInRecipe ingredient)
        {
            return  OneOrNull<UserDefinedIngredient>(await _data.LoadData<UserDefinedIngredient, dynamic>("SELECT * FROM UserDefinedIngredients WHERE IngredientID = @ingredientID", new { ingredientID = ingredient.IngredientID }, _config.GetConnectionString("recipeDatabase")));
        }
        private async Task DeleteIngredientInRecipe(uint RecipeID)
        {
            await _data.SaveData("DELETE FROM UserDefinedIngredientsInRecipe WHERE RecipeID = @recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<uint?> DeleteOnlyFile(string MD5Hash)
        {
            List<uint> result = await _data.LoadData<uint, dynamic>("SELECT RecipeID FROM FileManager WHERE FileID=@fileID", new { fileID = MD5Hash }, _config.GetConnectionString("recipeDatabase"));
            if(result != null && result.Count == 1)
            {
                await DeleteOnlyFile(result[0]);
                return result[0];
            }
            return null;
        }

        public async Task DeleteRecipeAndRelatedValues(uint RecipeID)
        {
            await DeleteMethod(RecipeID);
            await DeleteEquipment(RecipeID);
            await DeleteIngredientInRecipe(RecipeID);
            await DeleteOnlyRecipe(RecipeID);
            await DeleteOnlyFile(RecipeID);
            await DeleteOnlyReview(RecipeID);
        }
        private async Task DeleteOnlyFile(uint RecipeID)
        {
            await _data.SaveData("DELETE FROM FileManager WHERE RecipeID = @recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        private async Task DeleteOnlyRecipe(uint RecipeID)
        {
            await _data.SaveData("DELETE FROM Recipe WHERE RecipeID = @recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        private async Task DeleteOnlyReview(uint RecipeID)
        {
            await _data.SaveData("DELETE FROM Review WHERE RecipeID = @recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task DeleteReviewUsingReviewID(uint ReviewID)
        {
            await _data.SaveData("DELETE FROM Review WHERE ReviewID = @reviewID", new { reviewID = ReviewID }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task UpdateRecipe(Recipe NewModel)
        {
            await DeleteMethod(NewModel.RecipeID);
            await DeleteEquipment(NewModel.RecipeID);
            await DeleteIngredientInRecipe(NewModel.RecipeID);
            await _data.SaveData(NewModel.SqlUpdateStatement(), NewModel.SqlAnonymousType(NewModel.RecipeID), _config.GetConnectionString("recipeDatabase"));
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

        private async Task<List<Method>> GetMethod(uint RecipeID)
        {
            return await _data.LoadData<Method, dynamic>($"SELECT * FROM Method WHERE RecipeID=@RecipeID;", new { RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        private async Task<List<Equipment>> GetEquipment(uint RecipeID)
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

        public async Task<uint> InsertEquipment(Equipment equipment)
        {
            List<uint> autoIncrementResult = await _data.LoadData<uint, dynamic>(equipment.SqlInsertStatement() + "SELECT LAST_INSERT_ID();", equipment.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
            return autoIncrementResult[0];
        }

        public async Task<List<UserDefinedIngredientInRecipe>> GetIngredientsInRecipe(uint RecipeID)
        {
            return await _data.LoadData<UserDefinedIngredientInRecipe, dynamic>($"SELECT * FROM UserDefinedIngredientsInRecipe WHERE RecipeID =@recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<List<uint>> GetRecipeIDFromIngredientID(int quantity, string unit, uint userDefinedIngredientID)
        {
            return await _data.LoadData<uint, dynamic>($"SELECT RecipeID FROM UserDefinedIngredientsInRecipe WHERE  Quanity=@quantity AND Unit=@unit AND IngredientID = @ingredientID", new { Quantity = quantity, Unit = unit, IngredientID = userDefinedIngredientID }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task SaveNewReview(Review review)
        {
            await _data.SaveData(review.SQLInsertStatement(), review.SQLAnonymousType(), _config.GetConnectionString("recipeDatabase"));
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
            if (results.Count == 1)
            {
                pageVisits = results[0];
                pageVisits++;
                await _data.SaveData($"UPDATE RecipeDatabase.Recipe SET PageVisits=@PageVisits,  LastRequested=@LastRequested  WHERE RecipeID=@RecipeID; ", new { PageVisits = pageVisits, LastRequested = MySQLTimeFormat(DateTime.Now), RecipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            }
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

        public async Task<uint> InsertRecipeAndRelatedFields(Recipe displayModel)
        {
            List<uint> autoIncrementResult = await _data.LoadData<uint, dynamic>(displayModel.SqlInsertStatement() + "SELECT LAST_INSERT_ID();", displayModel.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
            uint RecipeId = autoIncrementResult[0];
            displayModel.RecipeID = RecipeId;
            await InsertRelatedFields(displayModel);
            return RecipeId;
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
                foreach (UserDefinedIngredientInRecipe model in displayModel.Ingredients)
                {
                    if (model.IngredientID != default(uint))
                    {
                        await _data.SaveData(model.SqlInsertStatement(), model.SqlAnonymousType((uint)model.IngredientID, displayModel.RecipeID), _config.GetConnectionString("recipeDatabase"));
                    }
                }
            }
        }

        public async Task<uint> InsertIngredient(UserDefinedIngredient model)
        {
            List<uint> autoIncrementResult = await _data.LoadData<uint, dynamic>(model.SqlInsertStatement() + "SELECT LAST_INSERT_ID();", model.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
            return autoIncrementResult[0];
        }

        public async Task<IEnumerable<Equipment>> FindEquipmentLike(string text)
        {
            return await _data.LoadData<Equipment, dynamic>("SELECT *  FROM Equipment WHERE EquipmentName LIKE Concat('%',@Text,'%') LIMIT 20;", new { Text = text }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<uint> EquipmentCount(Equipment equipment)
        {
            List<uint> resulsts = await _data.LoadData<uint, dynamic>("SELECT Count(*) FROM Equipment WHERE EquipmentName LIKE @equipmentName ", new { equipmentName = equipment.EquipmentName }, _config.GetConnectionString("recipeDatabase"));
            return resulsts[0];
        }

        public async Task<IEnumerable<UserDefinedIngredient>> FindIngredients(string text)
        {
            int lowerBound;
            if (text.Length < 5)
            {
                lowerBound = 1;
            }
            else
            {
                lowerBound = text.Length - 4;
            }
            int upperbound = text.Length * 2 + 3;
            return await _data.LoadData<UserDefinedIngredient, dynamic>(@"SELECT IngredientName, IngredientID FROM UserDefinedIngredients WHERE MATCH(IngredientName) AGAINST(@Text IN NATURAL LANGUAGE MODE) > 0 AND CHAR_LENGTH(IngredientName) <= @upperBound AND CHAR_LENGTH(IngredientName) >= @lowerBound LIMIT 10;", new { Text = text,lowerBound = lowerBound, upperBound = upperbound }, _config.GetConnectionString("recipeDatabase")); ;
        }

        public async Task<string> GetIngredientName(uint ingredientID)
        {
            List<string> results = await _data.LoadData<string, dynamic>("SELECT IngredientName From UserDefinedIngredients WHERE IngredientID = @ingredientID", new { ingredientID = ingredientID }, _config.GetConnectionString("recipeDatabase"));
            return results[0];
        }

        public async Task<List<Recipe>> GetHomepageRecipes(int offset)
        {
            List<Recipe> datas = await _data.LoadData<Recipe, dynamic>($"SELECT * FROM Recipe ORDER BY PageVisits DESC LIMIT 20 OFFSET {offset}", new { }, _config.GetConnectionString("recipeDatabase"));
            return await BuildRecipeTreeFromDataModel(datas);
        }

        public async Task<Recipe> GetRecipeAndTree(uint RecipeID)
        {
            List<Recipe> result = await _data.LoadData<Recipe, dynamic>("SELECT * FROM Recipe WHERE RecipeID = @recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));
            if (result.Count == 1)
            {
                result = await BuildRecipeTreeFromDataModel(result);

                return result[0];
            }
            return null;
        }

        public async Task<List<uint>> GetSearchDatabaseTextFields(string searchText, int offset)
        {
            return await _data.LoadData<uint, dynamic>(RecipeSearch+Union +FullTextSearchWithoutLimit + QueryLimit + Terminator, new { searchText = searchText, offset = offset }, _config.GetConnectionString("recipeDatabase"));
        }

        private const string FullTextSearchWithoutLimit = @"SELECT  RecipeID FROM Method
WHERE MATCH(MethodText) AGAINST(@searchText IN NATURAL LANGUAGE MODE) > 0
UNION DISTINCT
SELECT RecipeID FROM Recipe
WHERE MATCH(Description) AGAINST(@searchText IN NATURAL LANGUAGE MODE) > 0
UNION DISTINCT
SELECT RecipeID FROM UserDefinedIngredientsInRecipe
INNER JOIN(
SELECT IngredientID FROM UserDefinedIngredients
WHERE MATCH(IngredientName) AGAINST (@searchText IN NATURAL LANGUAGE MODE)> 0
) AS T2 ON UserDefinedIngredientsInRecipe.IngredientID = T2.IngredientID
UNION DISTINCT
SELECT RecipeID FROM EquipmentInRecipe
INNER JOIN(
SELECT EquipmentID
FROM Equipment
WHERE MATCH(EquipmentName) AGAINST (@searchText IN NATURAL LANGUAGE MODE)> 0
) AS T2 ON EquipmentInRecipe.EquipmentID = T2.EquipmentID"; //Removed natural expansion  WITH QUERY EXPANSION

        private const string RecipeSearch = @"SELECT RecipeID FROM Recipe
WHERE MATCH(RecipeName) AGAINST(@searchText IN NATURAL LANGUAGE MODE) > 0
";
        private const string Union = @" UNION DISTINCT ";

        private const string QueryLimit = " LIMIT 20 OFFSET @offset";

        private const string BitwiseDieaterySearch = @"SELECT s.RecipeID
FROM   UserDefinedIngredientsInRecipe s
WHERE  s.IngredientID IN (SELECT IngredientID FROM UserDefinedIngredients WHERE TypeOf & @bitPattern = 0 AND TypeOf!=0) AND s.RecipeID IN (" + FullTextSearchWithoutLimit + @")
GROUP BY s.RecipeID
" + QueryLimit +Terminator;

        private const string Terminator = ";";

        public async Task<List<uint>> GetSearchDatabaseTextFields(string searchText, int offset, ushort invertedTypeOfBitPattern)
        {
            Console.WriteLine(invertedTypeOfBitPattern);
            return await _data.LoadData<uint, dynamic>(BitwiseDieaterySearch + Terminator, new { searchText = searchText, offset = offset, bitPattern = invertedTypeOfBitPattern }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task RunSql(string sql)
        {
            await _data.SaveData(sql, new { }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<User> GetUserFromDatabase(string userName)
        {
            List<User> users = await _data.LoadData<User, dynamic>("SELECT * FROM Users WHERE UserName = @userName", new { userName = userName }, _config.GetConnectionString("recipeDatabase"));
            if (users.Count == 1)
            {
                return users[0];
            }
            return null;
        }

        public async Task SaveUser(User user)
        {
            await _data.SaveData(user.SqlInsertStatement(), user.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _data.LoadData<User, dynamic>("SELECT * FROM Users LIMIT 300", new { }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<List<Recipe>> BulkImportRecipes(int offset)
        {
            return await _data.LoadData<Recipe, dynamic>("SELECT * FROM Recipe LIMIT 300 OFFSET @offset ;", new { offset = offset }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task DeleteUser(string username)
        {
            await _data.SaveData("DELETE FROM Users WHERE UserName = @username", new { username = username }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task UpdatePassword(User user)
        {
            await _data.SaveData(user.SqlUpdateStatement(), user.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
        }

        public async Task InsertFile(FileManagerModel fileManagerModel)
        {
            await _data.SaveData(fileManagerModel.SqlInsertStatement(), fileManagerModel.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<FileManagerModel> GetFile(string MD5)
        {
            List<FileManagerModel> results = await _data.LoadData<FileManagerModel, dynamic>("SELECT * FROM FileManager WHERE FileID=@fileID", new { fileID = MD5 }, _config.GetConnectionString("recipeDatabase"));
            return OneOrNull<FileManagerModel>(results);
        }

        public async Task<FileManagerModel> GetFile(uint RecipeID)
        {
            List<FileManagerModel> results = await _data.LoadData<FileManagerModel, dynamic>("SELECT * FROM FileManager WHERE RecipeID=@recipeID", new { recipeID = RecipeID }, _config.GetConnectionString("recipeDatabase"));

            return OneOrNull<FileManagerModel>(results);
        }

        public async Task<List<FileManagerModel>> BulkImportFiles(int offset)
        {
            return await _data.LoadData<FileManagerModel, dynamic>("SELECT * FROM FileManager LIMIT 300 OFFSET @offset ;", new { offset = offset }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task GenericInsert<T>(T objectToInsert) where T: ISqlInsertible
        {
            await _data.SaveData(objectToInsert.SqlInsertStatement(), objectToInsert.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
        }

        public async Task GenericUpdate<T>(T objectToInsert) where T : ISqlUpdatible
        {
            await _data.SaveData(objectToInsert.SqlUpdateStatement(), objectToInsert.SqlAnonymousType(), _config.GetConnectionString("recipeDatabase"));
        }
        public async Task GenericDelete<T>(T objectToInsert) where T : ISqlDeletible
        {
            await _data.SaveData(objectToInsert.SqlDeleteStatement(), objectToInsert.SqlDeleteAnonymousType(), _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<PasswordRestToken> GetPasswordResetToken(string ResetTokenID)
        {
            List<PasswordRestToken> passwordRestTokens = await _data.LoadData<PasswordRestToken, dynamic>("SELECT * FROM PasswordResetToken WHERE ResetTokenID = @resetTokenID", new { ResetTokenID = ResetTokenID }, _config.GetConnectionString("recipeDatabase"));
            return OneOrNull<PasswordRestToken>(passwordRestTokens);
        }

        public async Task UpdateRecoveryEmailAddress(string oldEmailAddress, string newEmailAddress, string userName)
        {
            await _data.SaveData("UPDATE RecoveryEmailAddress SET EmailAddress = @newEmailAddress, UserName = @userName  WHERE EmailAddress = @oldEmailAddress", new { newEmailAddress = newEmailAddress, oldEmailAddress = oldEmailAddress, userName = userName }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<List<RecoveryEmailAddresses>> GetEmailAddresses()
        {
            return await _data.LoadData<RecoveryEmailAddresses, dynamic>("SELECT * FROM RecoveryEmailAddress", new { }, _config.GetConnectionString("recipeDatabase"));
        }

        public async Task<RecoveryEmailAddresses> GetSingleAddress(string email)
        {
            return OneOrNull<RecoveryEmailAddresses>(await _data.LoadData<RecoveryEmailAddresses, dynamic>("SELECT * FROM RecoveryEmailAddress WHERE EmailAddress=@email", new {email=email }, _config.GetConnectionString("recipeDatabase")));
        }

        

        /// <summary>
        /// Method that returns the first item in a list- intended for queries where you know there will only be one result. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        static T OneOrNull<T>(List<T> list)
        {
            if( list != null &&list.Count == 1)
            {
                return list[0];
            }
            else
            {
                return default;
            }
        } 
    }
}