using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class InventoryItem
    {

        public string EquipmentId { get; set; }
        public int Quantity { get; set; }
        public int Reserved { get; set; }
        public InventoryItem() { }

        public InventoryItem(string equipment, int quantity, int reserved)
        {
            this.EquipmentId = equipment;
            this.Quantity = quantity;
            this.Reserved = reserved;
        }
    }
}
