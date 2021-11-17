using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class Equipment
    {
        [Required]
        public string EquipmentName { get; set; }
        public uint EquipmentID { get; set; }
        [Required]
        public string TypeOf { get; set; }
        public static readonly List<string> Types = new List<string> { "Food Preparation Equipment", "Serving Equipment", "Cooking Equipment", "Storage Equipment", "Miscellaneous Equipment" };

        public string SqlInsertStatement()
        {
            return $"INSERT INTO Equipment(EquipmentName,TypeOf) VALUES(@equipmentName,@typeOf);";
        }

        public dynamic SqlAnonymousType()
        {
            return new { equipmentName = EquipmentName, typeOf = TypeOf };
        }
    }
}
