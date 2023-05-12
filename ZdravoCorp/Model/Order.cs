using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Storage;
using System.Windows;
using System.Timers;

namespace ZdravoCorp.Model
{
    public enum OrderStatus
    {
        InProcess,
        Done
    }
    public class Order 
    {
        public string Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public Order() {}
        public Order(MainStorage mainStorage) {
            this.Id = this.NextId(mainStorage);
            this.OrderStatus = OrderStatus.InProcess;
            this.TimeSlot = new TimeSlot();
            this.TimeSlot.StartTime = DateTime.Now;
            this.TimeSlot.EndTime = DateTime.Now.AddHours(24);
            this.OrderItems = new List<OrderItem>();
        }

        public Order(OrderStatus orderStatus, DateTime date, List<OrderItem> orderItems, MainStorage mainStorage)
        {
            this.Id = this.NextId(mainStorage);
            this.OrderStatus = orderStatus;
            this.TimeSlot = new TimeSlot();
            this.TimeSlot.StartTime = DateTime.Now;
            this.TimeSlot.EndTime = DateTime.Now.AddHours(24);
            this.OrderItems = orderItems;
            
        }

        public string NextId(MainStorage mainStorage)
        {
            string id = "";
            int lastId;

            if (mainStorage.Orders.Count() == 0)
            {
                lastId = 0;
            }
            else
            {
                foreach (Order orderStorage in mainStorage.Orders)
                {
                    id = orderStorage.Id;
                }

                lastId = int.Parse(System.Text.RegularExpressions.Regex.Replace(id, @"[^\d]+", ""));

            }
            return ("order" + (lastId + 1).ToString());

        }

        public void ChangeFileAfter24Hours(MainStorage mainStorage)
        {

            foreach(Order order in mainStorage.Orders)
            {
                if(order.OrderStatus == OrderStatus.InProcess)
                {
                    TimeSpan timeSpan = DateTime.Now - order.TimeSlot.EndTime;
                    if(timeSpan.TotalSeconds >= 0)
                    {
                        this.UpdateInventoryItems(mainStorage, order);
                    }
                }
            }
        }

        private void UpdateInventoryItems(MainStorage MainStorage, Order order)
        {

            foreach (OrderItem itemOrder in order.OrderItems)
            {
                bool findItem = false;

                for (int i = 0; i < MainStorage.Hospitals.Count; i++)
                {
                    for (int j = 0; j < MainStorage.Hospitals[i].Warehouses.Count; j++)
                    {
                        for (int k = 0; k < MainStorage.Hospitals[i].Warehouses[j].InventoryItems.Count; k++)
                        {
                            if (itemOrder.Equipment.Id.Equals(MainStorage.Hospitals[i].Warehouses[j].InventoryItems[k].EquipmentId))
                            {
                                MainStorage.Hospitals[i].Warehouses[j].InventoryItems[k].Quantity = itemOrder.Quantity + MainStorage.Hospitals[i].Warehouses[j].InventoryItems[k].Quantity;
                                findItem = true;
                                break;
                            }
                        }
                    }
                }

                if (findItem == false)
                {
                    InventoryItem inventoryItem = new InventoryItem(itemOrder.Equipment.Id, itemOrder.Quantity, 0);
                    MainStorage.Hospitals[0].Warehouses[0].InventoryItems.Add(inventoryItem);
                }
            }

            foreach(Order orderStorage in MainStorage.Orders)
            {
                if(orderStorage.Id == order.Id)
                {
                    orderStorage.OrderStatus = OrderStatus.Done;
                }
            }

            MainStorage.orderStorage.Save(MainStorage.Orders);
            MainStorage.hospitalStorage.Save(MainStorage.Hospitals);
            
        }
    }
}
