using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
     interface IRecipeDataModel
    {
        abstract List<RecipeDataModel> ParseFrontEndToBackend(List<DisplayRecipeModel> frontEndModel);
    }
}
