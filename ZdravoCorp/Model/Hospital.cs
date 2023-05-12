using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace ZdravoCorp.Model
{
    public class Hospital
    {
        public List<Warehouse> Warehouses { get; set; }
        public List<Room> Rooms { get; set; }

        public Hospital() { }
        public Hospital(List<Warehouse> warehouses, List<Room> rooms) {
            this.Warehouses = warehouses;
            this.Rooms = rooms;
        }

    }
}
