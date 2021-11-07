using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class EquipmentDataModel
    {
        public uint EquipmentID { get; set; }
        public SQLText EquipmentName { get; set; }
        public string TypeOf { get; set; }

        public readonly List<string> Types = new List<string> { "Food Preparation Equipment", "Serving Equipment", "Cooking Equipment", "Storage Equipment", "Miscellaneous Equipment" };
    }
}
