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

        public string SqlInsertStatement()
        {
            return $"INSERT INTO Equipment(EquipmentName) VALUES(@equipmentName);"; 
        }

        public dynamic SqlAnonymousType()
        {
            return new { equipmentName = EquipmentName };
        }
    }
}
