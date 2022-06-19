using BlazorServerApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipeController : Controller
    {
        private readonly IRecipeLoader _recipeDataLoader;

        public RecipeController(IRecipeLoader recipeLoader)
        {
            _recipeDataLoader = recipeLoader;
        }

       [HttpGet(Name = "RecipeList")]
        public IEnumerable<Recipe> Get()
        {
            return _recipeDataLoader.GetAllRecipes();
        }
    }
}
