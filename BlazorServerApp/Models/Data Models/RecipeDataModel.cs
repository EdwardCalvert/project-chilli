using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlazorServerApp.Models
{
    public class RecipeDataModel
    {
        public uint RecipeID;
        public uint Servings;
        public enum MEALTYPE
        {
            Starter,
            Main,
            Dessert,
            Snack,
            Accompaniment,
            Cake,
            Biscuit,
            LightMeal,
        }
        public MEALTYPE MealType;
        public string RecipeName;
        public float Kcal;
        public float Saturates;
        public float Carbohydrates;
        public float Sugar;
        public float Fibre;
        public float Protein;
        public float Salt;
        public float Fat;
        public uint CookingTime;
        public uint PreperationTime;
        public string DocxFilePath;

        public enum DIFFICULTY
        {
            Easy,
            Medium,
            Hard
        }
        public DIFFICULTY Difficulty;
        public uint PageVisits;
        public DateTime LastRequested;
        public string Description;




        public RecipeDataModel()
        {

        }
        public static RecipeDataModel ParseFrontEndToBackend(DisplayRecipeModel model)
        {
            RecipeDataModel dataModel = new RecipeDataModel();
            dataModel.CookingTime = (uint)model.CookingTime;
            dataModel.Servings = (uint)model.Servings;
            dataModel.MealType = (MEALTYPE)model.MealType;
            dataModel.RecipeName = model.RecipeName;
            dataModel.CookingTime = (uint)model.CookingTime;
            dataModel.PreperationTime = (uint)model.CookingTime;
            dataModel.DocxFilePath = model.DocxFilePath;
            dataModel.PageVisits = 1;
            dataModel.LastRequested = DateTime.Now;
            dataModel.Description = model.Description;
            return dataModel;
        }

        

        public static List<RecipeDataModel> ParseFrontEndToBackend(List<DisplayRecipeModel> frontEndModel)
        {
            List<RecipeDataModel> recipeDataModels = new List<RecipeDataModel>(frontEndModel.Count);
            foreach (DisplayRecipeModel model in frontEndModel)
            {
                recipeDataModels.Add(ParseFrontEndToBackend(model));
            }
            return recipeDataModels;
        }

        public string SqlInsertStatement()
        {
            return $"insert into Recipe values({RecipeID},{Servings},'{MealType}','{RecipeName}',{Kcal},{Saturates},{Carbohydrates},{Sugar},{Fibre},{Protein},{Salt},{Fat},{CookingTime},{PreperationTime},'{DocxFilePath}','{Difficulty}',{PageVisits},'{LastRequested.ToString("yyyy-MM-dd HH:mm:ss")}','{Description}')";
        }
    }
}
