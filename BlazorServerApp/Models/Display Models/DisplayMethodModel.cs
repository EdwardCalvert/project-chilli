using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class DisplayMethodModel
    {
        public string MethodText { get; set; }

        public uint StepNumber { get; set; }

        public uint RecipeID { get; set; }

        public DisplayMethodModel()
        {

        }

        public string SqlInsertStatement()
        {
            return $"INSERT INTO Method (StepNumber,RecipeID,MethodText) VALUES(@stepNumber, @recipeID,@methodText)";
        }

        public dynamic SqlAnonymousType()
        {
            return new { stepNumber = StepNumber, recipeID = RecipeID, methodText = MethodText };
        }
    }
}
