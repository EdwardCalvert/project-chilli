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
        

        public string SqlInsertStatement()
        {
            return $"insert into Recipe values({RecipeID},{Servings},'{MealType}','{RecipeName}',{Kcal},{Saturates},{Carbohydrates},{Sugar},{Fibre},{Protein},{Salt},{Fat},{CookingTime},{PreperationTime},'{DocxFilePath}','{Difficulty}',{PageVisits},'{LastRequested.ToString("yyyy-MM-dd HH:mm:ss")}','{Description}')";
        }
    }
}
