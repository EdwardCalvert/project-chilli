using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorServerApp.Extensions;

namespace BlazorServerApp.Models
{
    public class MethodDataModel
    {
        public uint StepNumber;
        public uint RecipeID;
        public string MethodText;

        public string SqlInsertStatement()
        {
            return $"INSERT INTO Method (StepNumber,RecipeID,MethodText) VALUES(@stepNumber, @recipeID,@methodText)"; 
        }

        public dynamic SqlAnonymousType()
        {
            return new { stepNumber = StepNumber, recipeID = RecipeID, methodText = MethodText};
        }
    }
}
