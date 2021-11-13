using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class EquipmentInRecipe
    {
        public uint EquipmentID { get; set; }
        public uint RecipeID { get; set; }

        public string SqlInsertStatement()
        {
            return $"INSERT INTO EquipmentInRecipe(EquipmentID,RecipeID) VALUES(@equipmentID, @recipeID);";
        }

        public dynamic SqlAnonymousType()
        {
            return new { equipmentID = EquipmentID, recipeID = RecipeID};
        }

        //public string SqlDeleteStatement()
        //{
        //    return "DELETE FROM `RecipeDatabase`.`EquipmentInRecipe` WHERE  `EquipmentID`=@equipmentID AND `RecipeID`=@recipeID;";
        //}

        //public dynamic SqlDeleteAnonymousType()
        //{
        //    return new {equipmentID = EquipmentID, recipeID = RecipeID };
        //}
    }
}
