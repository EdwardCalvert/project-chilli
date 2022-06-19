using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    public class Equipment
    {
        [Required]
        [MaxLength(DatabaseConstants.TinyTextMax)]
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