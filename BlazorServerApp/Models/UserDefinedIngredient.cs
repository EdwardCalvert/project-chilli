using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class UserDefinedIngredient
    {
         public uint? IngredientID { get; set; }
        public string IngredientName { get; set; }


        public string SqlInsertStatement()
        {
            return "INSERT INTO UserDefinedIngredients(IngredientName) VALUES(@ingredientName);";
        }

        public dynamic SqlAnonymousType()
        {
            return new { ingredientName = IngredientName};
        }
    }
}
