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
        public double Kcal;
        public double Saturates;
        public double Carbohydrates;
        public double Sugar;
        public double Fibre;
        public double Protein;
        public double Salt;
        public double Fat;
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
            return $"INSERT INTO Recipe(Servings, MealType, RecipeName, Kcal, Saturates, Carbohydrates, Sugar, Fibre, Protein, Salt, Fat, CookingTime, PreperationTime, Difficulty, PageVisits,LastRequested, Description)" +
                $"VALUES( @servings, @mealType, @recipeName, @kcal, @saturates, @carbohydrates, @sugar, @fibre, @protein, @salt, @fat, @cookingTime, @preperationTime, @difficulty, @pageVisits, @lastRequested, @description)";
                
        }

        public dynamic SqlAnonymousType()
        {
            return new { servings = Servings, mealType = MealType, recipeName = RecipeName, kcal = Kcal, saturates = Saturates, carbohydrates = Carbohydrates, sugar = Sugar, fibre= Fibre, protein = Protein, salt=Salt, fat=Fat, cookingTime = CookingTime, preperationTime = PreperationTime, difficulty = Difficulty, pageVisits = PageVisits, lastRequested = LastRequested, description = Description };
        }
    }
}
