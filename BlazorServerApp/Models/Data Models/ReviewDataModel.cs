using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorServerApp.Extensions;

namespace BlazorServerApp.Models
{
    public class ReviewDataModel
    {
        public uint ReviewID { get; set; }
        public uint RecipeID { get; set; }
        public string ReviewersName { get; set; }
        public string ReviewTitle { get; set; }
        public string ReviewText { get; set; }
        public int StarCount { get; set; }
        public DateTime DateSubmitted { get; set; }
        
        public string SQLInsertStatement()
        {
            return $"INSERT INTO Review(RecipeID,ReviewersName,ReviewTitle,ReviewText,StarCount,DateSubmitted)  VALUES({RecipeID},'{ReviewersName.MakeSQLSafe()}','{ReviewTitle.MakeSQLSafe()}','{ReviewText.MakeSQLSafe()}',{StarCount},'{RecipeDataLoader.MySQLTimeFormat(DateSubmitted)}');";
        }
    }
}
