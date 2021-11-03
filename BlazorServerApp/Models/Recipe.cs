using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class Recipe
    {
        private int RecipeID;
        enum MealType
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
        public uint Kcal;
        public uint Saturates;
        public uint Carbohydrates;
        public uint Sugar;
        public uint Fibre;
        public uint Protein;
        public uint Salt;

        private List<string> _ingredients;
        private int _cookingTime;
        private int _prepTime;
        private int _numberOfServings;
        private string _title;
        private MealType _mealType;

        public Recipe()
        {

        }
    }
}
