using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class OrderItem
    {
        public Equipment Equipment { get; set; }
        public int Quantity { get; set; }

        public OrderItem() {}

        public OrderItem(Equipment equipment, int quantity)
        {
            this.Quantity = quantity;
            this.Equipment = equipment;
        }
         

    }
}
