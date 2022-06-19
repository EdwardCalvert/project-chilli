
using BlazorServerApp.Models;

namespace APIServer
{
    public interface IRecipeLoader
    {

        public List<Recipe> GetAllRecipes();
    }
}
