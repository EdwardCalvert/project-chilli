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
            return new { equipmentID = EquipmentID, recipeID = RecipeID };
        }
    }
}