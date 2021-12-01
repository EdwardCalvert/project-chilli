using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    /// <summary>
    /// This class is intended as a front-end model, to allow data validation.
    /// </summary>
    public class Recipe
    {
        public const int INGREDIENTSCAPACITY = 30;
        public const int EQUIPMENTCAPACITY = 30;
        public const int METHODCAPACITY = 30;

        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        [MinLength(1, ErrorMessage = "Name is too short")]
        public string RecipeName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public List<Method> Method { get; set; } = new List<Method>();

        [Required]
        public IList<Equipment> Equipment { get; set; }

        [Range(1, 100)]
        public int Servings { get; set; }

        [Range(1, 1000)]
        public int CookingTime { get; set; }

        [Required, Range(1, 1000)]
        public int PreperationTime { get; set; }

        [Required, ValidMealType]
        public string MealType { get; set; }

        public uint RecipeID { get; set; }

        public static readonly List<string> mealType = new List<string>
        {
            "Starter",
            "Main",
            "Dessert",
            "Snack",
            "Accompaniment",
            "Cake",
            "Biscuit",
            "Light Meal",
        };

        [Required, ValidIngredientsInRecipe]
        public List<UserDefinedIngredientInRecipe> Ingredients { get; set; } = new List<UserDefinedIngredientInRecipe>();

        public bool ManualUpload { get; set; }

        public DateTime LastRequested { get; set; }

        public static readonly List<string> SUPPORTEDUNITS = new List<string>{
            "x",
            "Grams",
            "Cup",
            "Litre",
            "Mililetres",
            "Fluid Ounces",
            "Gallon",
            "Quart",
            "Pint",
            "Tsp",
            "Tbsp",
            "Dessert spoon",
            "Gallon",
            "Kilograms",
            "Pound",
            "Ounce",
            "Fluid Ounces",
            };

        public double Kcal { get; set; }
        public double Fat { get; set; }
        public double Saturates { get; set; }
        public double Sugar { get; set; }
        public double Fibre { get; set; }
        public double Carbohydrates { get; set; }
        public double Salt { get; set; }
        public double Protein { get; set; }

        public static Dictionary<string, int> DifficultyEnum = new Dictionary<string, int>()
        {
            { "Easy",1},
            {"Medium",2 },
            {"Hard",3 },
        };

        public static readonly List<string> DIFICULTY = new List<string>(Recipe.DifficultyEnum.Keys);

        public static RecomendedIntake RecomendedIntake = new RecomendedIntake(2000, 70, 20, 260, 90, 50, 6);

        [Required]
        public string Difficulty { get; set; }

        public List<Review> Reviews = new List<Review>();

        public DisplayNutritionModel DisplayNutritionModel { get; set; }

        public uint PageVisits { get; set; }

        public string SqlInsertStatement()
        {
            return $"INSERT INTO Recipe(Servings, MealType, RecipeName, Kcal, Saturates, Carbohydrates, Sugar, Fibre, Protein, Salt, Fat, CookingTime, PreperationTime, Difficulty, PageVisits,LastRequested, Description, ManualUpload)" +
                $"VALUES( @servings, @mealType, @recipeName, @kcal, @saturates, @carbohydrates, @sugar, @fibre, @protein, @salt, @fat, @cookingTime, @preperationTime, @difficulty, @pageVisits, @lastRequested, @description,@manualUpload); ";
        }

        public dynamic SqlAnonymousType()
        {
            return new { servings = Servings, mealType = MealType, recipeName = RecipeName, kcal = Kcal, saturates = Saturates, carbohydrates = Carbohydrates, sugar = Sugar, fibre = Fibre, protein = Protein, salt = Salt, fat = Fat, cookingTime = CookingTime, preperationTime = PreperationTime, difficulty = Difficulty, pageVisits = PageVisits, lastRequested = DateTime.Now, description = Description, manualUpload = ManualUpload };
        }

        public dynamic SqlAnonymousType(uint RecipeID)
        {
            return new { servings = Servings, mealType = MealType, recipeName = RecipeName, kcal = Kcal, saturates = Saturates, carbohydrates = Carbohydrates, sugar = Sugar, fibre = Fibre, protein = Protein, salt = Salt, fat = Fat, cookingTime = CookingTime, preperationTime = PreperationTime, difficulty = Difficulty, pageVisits = PageVisits, lastRequested = DateTime.Now, description = Description, recipeID = RecipeID };
        }

        public string SqlUpdateStatement()
        {
            return $"UPDATE Recipe SET Servings = @servings, MealType =  @mealType, RecipeName = @recipeName, Kcal = @kcal, Saturates = @saturates, Carbohydrates = @carbohydrates, Sugar = @sugar, Fibre = @fibre, Protein = @protein, Salt = @salt, Fat = @fat, CookingTime = @cookingTime, PreperationTime = @preperationTime, Difficulty = @difficulty, PageVisits = @pageVisits,LastRequested = @lastRequested, Description = @description WHERE RecipeID = @recipeID; ";
        }

        public void InsertEmptyIngredient(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                InsertEmptyIngredient();
            }
        }

        public void InsertEmptyIngredient()
        {
            Ingredients.Add(new UserDefinedIngredientInRecipe());
        }

        public void InsertEmptyMethod()
        {
            Method.Add(new Method());
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