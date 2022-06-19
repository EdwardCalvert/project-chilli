
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
        public Recipe()
        {
        }
        public const int INGREDIENTSCAPACITY = 30;
        public const int EQUIPMENTCAPACITY = 30;
        public const int METHODCAPACITY = 30;

        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        [MinLength(1, ErrorMessage = "Name is too short")]
        public string RecipeName { get; set; }

        [Required]
        [MaxLength(DatabaseConstants.VarCharMax,ErrorMessage ="Please shorten the description - too long for database")]
        public string Description { get; set; }

        [ValidateComplexType,Required,ListLengthGreaterThanZero]
        public List<Method> Method { get; set; } = new List<Method>();

        [ValidateComplexType]
        public IList<Equipment> Equipment { get; set; }

        [Range(1, 100),Required]
        public int Servings { get; set; }

        [Range(0, 1000)]
        public int CookingTime { get; set; }

        [Required, Range(0, 1000)]
        public int PreperationTime { get; set; }


        public uint RecipeID { get; set; }


        [ValidateComplexType,Required, ListLengthGreaterThanZero]
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public bool ManualUpload { get; set; }

        public DateTime LastRequested { get; set; }




        public static Dictionary<string, int> DifficultyEnum = new Dictionary<string, int>()
        {
            { "Easy",1},
            {"Medium",2 },
            {"Hard",3 },
        };

        [ValidDifficulty]
        public static readonly List<string> DIFICULTY = new List<string>(DifficultyEnum.Keys);


        [Required]
        public string Difficulty { get; set; }

        public List<Review> Reviews = new List<Review>();


        public uint PageVisits { get; set; }

        public string SqlInsertStatement()
        {
            return $"INSERT INTO Recipe(Servings, MealType, RecipeName, CookingTime, PreperationTime, Difficulty, PageVisits,LastRequested, Description, ManualUpload)" +
                $"VALUES( @servings, @mealType, @recipeName, @cookingTime, @preperationTime, @difficulty, @pageVisits, @lastRequested, @description,@manualUpload); ";
        }

        public dynamic SqlAnonymousType()
        {
            return new { servings = Servings,  recipeName = RecipeName, cookingTime = CookingTime, preperationTime = PreperationTime, difficulty = Difficulty, pageVisits = PageVisits, lastRequested = DateTime.Now, description = Description, manualUpload = ManualUpload };
        }

        public dynamic SqlAnonymousType(uint RecipeID)
        {
            return new { servings = Servings,  recipeName = RecipeName, cookingTime = CookingTime, preperationTime = PreperationTime, difficulty = Difficulty, pageVisits = PageVisits, lastRequested = DateTime.Now, description = Description, recipeID = RecipeID };
        }

        public string SqlUpdateStatement()
        {
            return $"UPDATE Recipe SET Servings = @servings, MealType =  @mealType, RecipeName = @recipeName,CookingTime = @cookingTime, PreperationTime = @preperationTime, Difficulty = @difficulty, PageVisits = @pageVisits,LastRequested = @lastRequested, Description = @description WHERE RecipeID = @recipeID; ";
        }



        public void InsertEmptyIngredient()
        {
            Ingredients.Add(new Ingredient());
        }

        public void InsertEmptyMethod()
        {
            Method.Add(new Method());
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