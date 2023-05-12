using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ZdravoCorp.Storage;

namespace ZdravoCorp.Model
{
    public enum TransferType
    {
        Deferred,
        Instant
    }

    public enum TransferStatus
    {
        InProcess,
        Done
    }
    public class Transfer
    {
        public string Id { get; set; }
        public string FromRoomId { get; set; }
        public string ToRoomId { get; set; }
        public List<InventoryItem> InventoryItems { get; set; }
        public TransferType TransferType { get; set; }
        public TransferStatus TransferStatus { get; set; }
        public TimeSlot TimeSlot { get; set; }

        public Transfer() { }
        public Transfer(MainStorage mainStorage, string fromRoomId, string toRoomId, TransferType type)
        {
            this.Id = NextId(mainStorage);
            this.FromRoomId = fromRoomId;
            this.ToRoomId = toRoomId;
            this.InventoryItems = new List<InventoryItem>();
            this.TransferType = type;
            this.TransferStatus = TransferStatus.InProcess;

            this.TimeSlot = new TimeSlot();
            this.TimeSlot.StartTime = DateTime.Now;
            if (this.TransferType == TransferType.Instant)
            {
                this.TimeSlot.EndTime = DateTime.Now;
            }
        }

        public Transfer(string id, string fromRoomId, string toRoomId, List<InventoryItem> inventoryItems, TransferType transferType, TimeSlot timeSlot)
        {
            this.Id = id;
            this.FromRoomId = fromRoomId;
            this.ToRoomId = toRoomId;
            this.InventoryItems = inventoryItems;
            this.TransferType = transferType;
            this.TimeSlot = timeSlot;
        }

        public string NextId(MainStorage mainStorage)
        {
            string id = "";
            int lastId;

            if (mainStorage.Transfers.Count() == 0)
            {
                lastId = 0;
            }
            else
            {
                foreach (Transfer transferStrorage in mainStorage.Transfers)
                {
                    id = transferStrorage.Id;
                }

                lastId = int.Parse(System.Text.RegularExpressions.Regex.Replace(id, @"[^\d]+", ""));

            }
            return ("transfer" + (lastId + 1).ToString());

        }

        public void ChangeFileDeferred(MainStorage mainStorage)
        {

            foreach (Transfer transfer in mainStorage.Transfers)
            {
                if (transfer.TransferType == TransferType.Deferred && transfer.TransferStatus == TransferStatus.InProcess)
                {
                    TimeSpan timeSpan = DateTime.Now - transfer.TimeSlot.EndTime;
                    if (timeSpan.TotalSeconds >= 0)
                    {
                        this.UpdateInventoryItems(mainStorage, transfer);
                    }
                }
            }
        }

        public void UpdateInventoryItems(MainStorage MainStorage, Transfer transfer)
        {

            foreach (InventoryItem itemTransfer in transfer.InventoryItems)
            {
                bool findItem = false;

                for (int i = 0; i < MainStorage.Hospitals.Count; i++)
                {
                    foreach (Warehouse warehouse in MainStorage.Hospitals[i].Warehouses)
                    {
                        if (warehouse.Id == transfer.FromRoomId) //brisanje reserved-a
                        {
                            foreach (InventoryItem itemWarehouse in warehouse.InventoryItems)
                            {
                                if (itemWarehouse.EquipmentId == itemTransfer.EquipmentId)
                                {
                                    itemWarehouse.Reserved = itemWarehouse.Reserved - itemTransfer.Reserved;

                                }
                            }
                        }

                        if (warehouse.Id == transfer.ToRoomId) //dodavanje na quantity
                        {
                            foreach (InventoryItem itemWarehouse in warehouse.InventoryItems)
                            {

                                if (itemWarehouse.EquipmentId == itemTransfer.EquipmentId)
                                {
                                    findItem = true;
                                    itemWarehouse.Quantity = itemWarehouse.Quantity + itemTransfer.Reserved;

                                }
                            }
                        }
                    }

                    foreach (Room room in MainStorage.Hospitals[i].Rooms)
                    {
                        {
                            if (room.Id == transfer.FromRoomId) //brisanje reserved-a
                            {
                                foreach (InventoryItem itemWarehouse in room.InventoryItems)
                                {
                                    if (itemWarehouse.EquipmentId == itemTransfer.EquipmentId)
                                    {
                                        itemWarehouse.Reserved = itemWarehouse.Reserved - itemTransfer.Reserved;
                                    }
                                }
                            }

                            if (room.Id == transfer.ToRoomId) //dodavanje na quantity
                            {
                                foreach (InventoryItem itemWarehouse in room.InventoryItems)
                                {
                                    if (itemWarehouse.EquipmentId == itemTransfer.EquipmentId)
                                    {
                                        findItem = true;
                                        itemWarehouse.Quantity = itemWarehouse.Quantity + itemTransfer.Reserved;
                                    }
                                }
                            }
                        }
                    }
                }

                if (findItem == false)
                {
                    InventoryItem newInventoryItem = new InventoryItem(itemTransfer.EquipmentId, itemTransfer.Reserved, 0);

                    foreach (Warehouse warehouse in MainStorage.Hospitals[0].Warehouses)
                    {
                        if (warehouse.Id == transfer.ToRoomId)
                        {
                            warehouse.InventoryItems.Add(newInventoryItem);
                        }
                    }

                    foreach (Room room in MainStorage.Hospitals[0].Rooms)
                    {
                        if (room.Id == transfer.ToRoomId)
                        {
                            room.InventoryItems.Add(newInventoryItem);
                        }
                    }
                }

            }

            foreach (Transfer tranferStorage in MainStorage.Transfers)
            {
                if (tranferStorage.Id == transfer.Id)
                {
                    tranferStorage.TransferStatus = TransferStatus.Done;
                }
            }

            MainStorage.transferStorage.Save(MainStorage.Transfers);
            MainStorage.hospitalStorage.Save(MainStorage.Hospitals);
        }
    }

}
