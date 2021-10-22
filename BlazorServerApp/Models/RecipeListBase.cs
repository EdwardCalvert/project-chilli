using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class RecipeListBase : ComponentBase
    {
        public IEnumerable<Recipe> Recipes { get; set; }
        public string Error;

        protected override Task OnInitializedAsync()
        {
            LoadHomepageRecipies();
            return base.OnInitializedAsync();
        }

        private void LoadHomepageRecipies()
        {
            //go to database, and get a list of the 'top' 100 recipes
            List<Recipe> recipes;
            try
            {
                //throw new Exception("This is a demonstration of an error");
            }
            catch (Exception exception)
            {
                Error = exception.Message;
            }
        }
    }

}
