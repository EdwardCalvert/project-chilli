using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BlazorServerApp.Models
{
    public class DisplayIngredientInRecipeModel 
    {
        [Required, Range(0.01, 1000)]
        public double Quantity { get; set; }

        [Required, ValidUnit]
        public string Unit { get; set; }
        
        [Required, ValidRecipeID]
        public uint? IngredientID { get; set; }

        public DisplayIngredientInRecipeModel(int quantity, string unit)
        {
            Quantity = quantity;
            Unit = unit;
        }

        public DisplayIngredientInRecipeModel() { }

       public string SqlInsertStatement()
        {
            return "INSERT INTO IngredientsInRecipe VALUES( @ingredientID, @recipeID, @quantity, @unit);";
        }

        public dynamic SqlAnonymousType(uint IngredientId, uint RecipeID)
        {
            return new { ingredientID = IngredientID, recipeID = RecipeID,quantity = Quantity, unit = Unit};
        }

    }



}
