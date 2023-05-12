using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public enum EquipmentType
    {
        Appointments,
        Surgeries,
        RoomFurniture,
        HallwayEquipments
    }
    public class Equipment 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public EquipmentType EquipmentType { get; set; }

        public Equipment() { }

        public Equipment(string id, string name, EquipmentType equipmentType)
        {
            this.Id = id;
            this.Name = name;
            this.EquipmentType = equipmentType;
        }

        
           
    }
}
