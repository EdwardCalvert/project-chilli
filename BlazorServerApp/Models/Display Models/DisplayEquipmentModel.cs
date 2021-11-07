using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class DisplayEquipmentModel
    {
        public string Name { get; set; }
        public uint EquipmentID { get; set; }
        public string TypeOf { get; set; }
        public static readonly List<string> Types = new List<string> { "Food Preparation Equipment", "Serving Equipment", "Cooking Equipment", "Storage Equipment", "Miscellaneous Equipment" };

        public DisplayEquipmentModel()
        {

        }
    }
}
