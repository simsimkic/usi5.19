using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class Warehouse
    {
        public string Id { get; set; }
        public List<InventoryItem> InventoryItems { get; set; }

        public Warehouse() { }

        public Warehouse(string id, List<InventoryItem> inventoryItems)
        {
            this.Id = id;
            this.InventoryItems = inventoryItems;
        }

    }
}
