using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class ReviewDataModel
    {
        public uint ReviewID { get; set; }
        public uint RecipeID { get; set; }
        public string ReviewersName { get; set; }
        public string ReviewerTitle { get; set; }
        public string ReviewText { get; set; }
        public int StarCount { get; set; }
    }
}
