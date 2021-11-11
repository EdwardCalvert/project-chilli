namespace BlazorServerApp.Models
{
    public interface ISqlInsertible
    {
         string SqlInsertStatement();

        dynamic SqlAnonymousType(uint RecipeID);
    }
}
