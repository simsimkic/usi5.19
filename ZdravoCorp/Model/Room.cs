using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public enum RoomType
    {
        OperatingRoom,
        ExaminationRoom,
        PatientRoom,
        WaitingRoom,
        WareHouse
    }
    public class Room
    {
        public string Id { get; set; }
        public int Capacity { get; set; }
        public int FreeBeds { get; set; }
        public RoomType RoomType { get; set; }
        public List<InventoryItem> InventoryItems { get; set; }

        public Room() { }

        public Room(string id, int capacity, int freeBeds, RoomType roomType, List<InventoryItem> inventoryItems)
        {
            this.Id = id;
            this.Capacity = capacity;
            this.FreeBeds = freeBeds;
            this.RoomType = roomType;
            this.InventoryItems = inventoryItems;
        }


    }
}
