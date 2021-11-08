using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace BlazorServerApp.Models
{
    /// <summary>
    /// This class is intended as a front-end model, to allow data validation.
    /// </summary>
    public class DisplayRecipeModel
    {
        public const int INGREDIENTSCAPACITY= 30;
        public const int EQUIPMENTCAPACITY = 30;
        public const int METHODCAPACITY = 30;

        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        [MinLength(1, ErrorMessage = "Name is too short")]
        public string RecipeName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public List<DisplayMethodModel> Method { get; set; } = new List<DisplayMethodModel>();

        [Required]
        public IEnumerable<DisplayEquipmentModel> Equipment { get; set; } = new List<DisplayEquipmentModel>();

        [Range(1, 100)]
        public int Servings { get; set; }

        [Range(1, 1000)]
        public int CookingTime { get; set; }

        [Required]
        [Range(1, 1000)]
        public int PreperationTime { get; set; }

        [Required]
        [EnumDataType(typeof(mealType))]
        public mealType MealType;

        public uint RecipeID { get; set; }

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

        [ValidIngredient]
        public List<DisplayIngredientModel> Ingredients = new List<DisplayIngredientModel>();

        public string DocxFilePath;


        public DateTime LastRequested { get; set; }

        public static readonly List<string> SUPPORTEDUNITS = new List<string>{
            "Grams",
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
            "Kilograms",
            "Pound",
            "Ounce",
            };

        public double Kcal { get; set; }
        public double Fat { get; set; }
        public double Saturates { get; set; }
        public double Sugar { get; set; }
        public double Fibre { get; set; }
        public double Carbohydrates { get; set; }
        public double Salt { get; set; }
        public double Protein { get; set; }

        public static readonly List<string> DIFICULTY = new List<string>
        {
            "Easy","Medium","Hard"
        };

        public string Difficulty { get; set; }

        public List<DisplayReviewModel> Reviews = new List<DisplayReviewModel>();

        public DisplayNutritionModel DisplayNutritionModel { get; set; }

        public uint PageVisits { get; set; }


        public string SqlInsertStatement()
        {
            return $"INSERT INTO Recipe(Servings, MealType, RecipeName, Kcal, Saturates, Carbohydrates, Sugar, Fibre, Protein, Salt, Fat, CookingTime, PreperationTime, Difficulty, PageVisits,LastRequested, Description)" +
                $"VALUES( @servings, @mealType, @recipeName, @kcal, @saturates, @carbohydrates, @sugar, @fibre, @protein, @salt, @fat, @cookingTime, @preperationTime, @difficulty, @pageVisits, @lastRequested, @description)";

        }

        public dynamic SqlAnonymousType()
        {
            return new { servings = Servings, mealType = MealType, recipeName = RecipeName, kcal = Kcal, saturates = Saturates, carbohydrates = Carbohydrates, sugar = Sugar, fibre = Fibre, protein = Protein, salt = Salt, fat = Fat, cookingTime = CookingTime, preperationTime = PreperationTime, difficulty = Difficulty, pageVisits = PageVisits, lastRequested = LastRequested, description = Description };
        }

        public void InsertEmptyIngredient(int quantity)
        {
            for(int i =0; i<quantity; i++)
            {
                InsertEmptyIngredient();
            }
        }

        public void InsertEmptyIngredient()
        {
            Ingredients.Add(new DisplayIngredientModel());
        }


        public void InsertEmptyMethod()
        {
            Method.Add(new DisplayMethodModel());
        }

        public void InsertEmptyMethod(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                InsertEmptyMethod();
            }
        }

        public string ShowShortDescription()
        {
            return ShowShortDescription(200);
        }

        public string ShowShortDescription(int length)
        {
            if (string.IsNullOrEmpty(Description))
            {
                return string.Empty;
            }

            // If text in shorter or equal to length, just return it
            if (Description.Length <= length)
            {
                return Description;
            }

            // Text is longer, so try to find out where to cut
            char[] delimiters = new char[] { ' ', '.', ',', ':', ';' };
            int index = Description.LastIndexOfAny(delimiters, length - 3);

            if (index > (length / 2))
            {
                return Description.Substring(0, index) + "...";
            }
            else
            {
                return Description.Substring(0, length - 3) + "...";
            }
        }
    }

  
}
