using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class Recipe
    {
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
        private List<string> _ingredients;
        private int _cookingTime;
        private int _prepTime;
        private int _numberOfServings;
        private string _title;
        private MealType _mealType;
        //private List<Equipment> equipment;

        //private NutritionLabel _nutritionLabel;
        //private Rating _rating;

        public Recipe()
        {

        }
    }
}
