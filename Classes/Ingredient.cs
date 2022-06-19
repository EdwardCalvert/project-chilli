
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class Ingredient
    {

        public Ingredient()
        {

        }

        public uint? IngredientID { get; set; }
        public uint RecipeID { get; set; }
        public string IngredientName { get; set; }


        public Type TypeOf { get; set; }


        public string SqlInsertStatement()
        {
            return "INSERT INTO Ingredient(RecipeID,IngredientName,TypeOf) VALUES(@RecipeID,@ingredientName,@TypeOf);";
        }

        public dynamic SqlAnonymousType()
        {
            return new { ingredientName = IngredientName, TypeOf = TypeOf , RecipeID = RecipeID,IngredientID = IngredientID};
        }
    }
}