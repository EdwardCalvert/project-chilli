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
    }
}
