using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class FileManagerModel
    {
        public string FileID { get; set; }
        public uint RecipeID { get; set; }
        public uint NumberOfViews { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime DateUploaded { get; set; }

        public string SqlInsertStatement()
        {
            return "INSERT INTO FileManager(FileID,RecipeID,NumberOfViews,LastAccessed,DateUploaded) " +
                                    "VALUES(@FileID,@recipeID,@numberOfViews,@lastAccessed,@dateUploaded)";
        }

        public dynamic SqlAnonymousType()
        {
            return new{ FileID = FileID,recipeID = RecipeID,numberOfViews = NumberOfViews, lastAccessed = LastAccessed, dateUploaded = DateUploaded };
        }
    }
}
