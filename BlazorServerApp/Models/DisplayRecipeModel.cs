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
        public List<DisplayMethodList> Method { get; set; } = new List<DisplayMethodList>();

        [Required]
        public List<DisplayEquipmentModel> Equipment { get; set; } = new List<DisplayEquipmentModel>();

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


        public static readonly List<string> DIFICULTY = new List<string>
        {
            "Easy","Medium","Hard"
        };

        public string Difficulty { get; set; }

        public List<DisplayReviewModel> Reviews = new List<DisplayReviewModel>();

        public static DisplayRecipeModel PasrseBackendToFrontend(RecipeDataModel recipeDataModel)
        {
            DisplayRecipeModel displayRecipeModel= new DisplayRecipeModel();
            displayRecipeModel.CookingTime = (int) recipeDataModel.CookingTime;
            displayRecipeModel.Servings = (int)recipeDataModel.Servings;
            displayRecipeModel.MealType = (mealType)recipeDataModel.MealType;
            displayRecipeModel.RecipeName = recipeDataModel.RecipeName;
            displayRecipeModel.CookingTime = (int) recipeDataModel.CookingTime;
            displayRecipeModel.PreperationTime = (int) recipeDataModel.PreperationTime;
            displayRecipeModel.DocxFilePath = recipeDataModel.DocxFilePath;
            displayRecipeModel.Description = recipeDataModel.Description;

            return displayRecipeModel;
        }

        public static List<DisplayRecipeModel> PasrseBackendToFrontend(List<RecipeDataModel> recipeDataModel)
        {
            List<DisplayRecipeModel> recipeModels = new();
            foreach(RecipeDataModel model in recipeDataModel)
            {
                recipeModels.Add(PasrseBackendToFrontend(model));
            }
            return recipeModels;
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

        public void InsertEmptyEquipment(int quantity)
        {
            for(int i =0; i <quantity; i++)
            {
                InsertEmptyEquipment();
            }
        }

        public void InsertEmptyEquipment()
        {
            Equipment.Add(new DisplayEquipmentModel());        
        }

        public void InsertEmptyMethod()
        {
            Method.Add(new DisplayMethodList());
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

    [AttributeUsage(AttributeTargets.Property |
  AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ValidIngredient : ValidationAttribute
    {
        public bool IsValid(DisplayRecipeModel value)
        {
            int count = 0;
            foreach(DisplayIngredientModel model in value.Ingredients)
            {
                if ((string.IsNullOrEmpty(model.Name) || model.Quantity is default(int) || model.Name == "Eric"))
                    {
                    return false;
                }
                count++;
            }
            return true;

        }
    }
}
