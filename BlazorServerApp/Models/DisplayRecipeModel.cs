using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    /// <summary>
    /// This class is intended as a front-end model, to allow data validation.
    /// </summary>
    public class DisplayRecipeModel
    {
        public const int INGREDIENTSCAPACITY= 30;

        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        [MinLength(1, ErrorMessage = "Name is too short")]
        public string RecipeName { get; set; }

        [Required]
        public string Description { get; set; }

        //[Required]
        //public string Ingredients { get; set; }

        public int Servings { get; set; }

        public int CookingTime { get; set; }

        [Required]
        public int PreperationTime { get; set; }

        public enum mealType
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

        public string Units { get; set; }

        public int[] quantities = new int[INGREDIENTSCAPACITY];

        public string[] ingredientsNames = new string[INGREDIENTSCAPACITY];

        public string[] unitsList = new string[INGREDIENTSCAPACITY];

        public int testField;

        public static readonly List<string> SUPPORTEDUNITS = new List<string>{
            "Cup",
            "Litre",
            "Mililetres",
            "Fluid Ounces",
            "Gallon",
            "Quart",
            "Pint",
            "Tsp",
            "Tspb",
            "Dessert spoon",
            "Gallon",
            "Grams",
            "Kilograms",
            "Pound",
            "Ounce"
            };

    

        [Required]
        public mealType MealType;

    }
}
