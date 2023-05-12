using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Xceed.Wpf.Toolkit.Core.Converters;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class DistributionOrderViewModel : ViewModelBase
    {
        public Transfer Transfer { get; set; }
        public List<Tuple<Equipment, int>> EquipmentsFromRoom { get; set; }
        public List<Tuple<Equipment, int>> EquipmentsToRoom { get; set; }
        public int? QuantityInput { get; set; }
        public DateTime DateInput { get; set; }
        public bool IsDatePickerEnabled => this.Transfer.TransferType != TransferType.Instant;
        public ICommand SaveOrder { get; }
        public ICommand SubmitOrder { get; }
        public ICommand Back { get; }

        public MainStorage MainStorage { get; set; }

        private Tuple<Equipment, int> _selectedRow;
        public Tuple<Equipment, int> SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                _selectedRow = value;
                OnPropertyChanged(nameof(SelectedRow));
            }
        }

        public DistributionOrderView DistributionOrderView { get; set; }
        public DistributionOrderViewModel() { }
        public DistributionOrderViewModel(MainStorage mainStorage, DistributionOrderView distributionOrderView, Transfer transfer) { 
            this.MainStorage = mainStorage;
            this.DistributionOrderView = distributionOrderView;
            this.SaveOrder = new RelayCommand(SaveOrderButton);
            this.SubmitOrder = new RelayCommand(SubmitOrderButton);
            this.Back = new RelayCommand(BackButton);

            this.Transfer = transfer;
            this.EquipmentsFromRoom = GetEquipmentsByRoom(transfer.FromRoomId);
            this.EquipmentsToRoom = GetEquipmentsByRoom(transfer.ToRoomId);
        }

        private void SaveOrderButton(object obj)
        {
            if (this.QuantityInput <= 0)
            {
                MessageBox.Show("You can't distribute less than 1 equipment!");
            }
            else if (this.QuantityInput > this.SelectedRow.Item2)
            {
                MessageBox.Show("You don't have as many as required eqipment in this room!");
            }
            else
            {
                InventoryItem inventoryItem = new InventoryItem(this.SelectedRow.Item1.Id, this.SelectedRow.Item2, (int)this.QuantityInput);

                bool isExist = false;
                for (int i = 0; i < this.Transfer.InventoryItems.Count; i++)
                {
                    if (this.Transfer.InventoryItems[i].EquipmentId == inventoryItem.EquipmentId)
                    {
                        this.Transfer.InventoryItems[i].Reserved = this.Transfer.InventoryItems[i].Reserved + (int)this.QuantityInput;
                        isExist = true; 
                        break;
                    }
                }

                if (isExist == false)
                {
                    this.Transfer.InventoryItems.Add(inventoryItem);
                }

                MessageBox.Show($"You just add {inventoryItem.EquipmentId} on transfer list!");
            }
        }

        public List<Tuple<Equipment, int>> GetEquipmentsByRoom(string roomId)
        {
            List<Tuple<Equipment, int>> equipments = new List<Tuple<Equipment, int>>();

            for (int i = 0; i < this.MainStorage.Hospitals.Count; i++)
            {
                foreach (Warehouse warehouse in this.MainStorage.Hospitals[i].Warehouses)
                {
                    if (roomId == warehouse.Id)
                    {
                        foreach (InventoryItem inventoryItem in warehouse.InventoryItems)
                        {
                            foreach (Equipment equipment in this.MainStorage.Equipments)
                            {
                                if (equipment.Id == inventoryItem.EquipmentId && ((this.Transfer.TransferType == TransferType.Instant && (equipment.EquipmentType == EquipmentType.Appointments || equipment.EquipmentType == EquipmentType.Surgeries)) || ((this.Transfer.TransferType == TransferType.Deferred && (equipment.EquipmentType == EquipmentType.RoomFurniture || equipment.EquipmentType == EquipmentType.HallwayEquipments)))))
                                {
                                    equipments.Add(new Tuple<Equipment, int>(equipment, inventoryItem.Quantity));
                                }
                            }
                        }
                    }
                }

                foreach (Room room in this.MainStorage.Hospitals[i].Rooms)
                {
                    if (roomId == room.Id)
                    {
                        foreach (InventoryItem inventoryItem in room.InventoryItems)
                        {
                            foreach (Equipment equipment in this.MainStorage.Equipments)
                            {
                                if (equipment.Id == inventoryItem.EquipmentId && ((this.Transfer.TransferType == TransferType.Instant && (equipment.EquipmentType == EquipmentType.Appointments || equipment.EquipmentType == EquipmentType.Surgeries)) || ((this.Transfer.TransferType == TransferType.Deferred && (equipment.EquipmentType == EquipmentType.RoomFurniture || equipment.EquipmentType == EquipmentType.HallwayEquipments)))))
                                {
                                    equipments.Add(new Tuple<Equipment, int>(equipment, inventoryItem.Quantity));
                                }
                            }
                        }
                    }
                }
            }

            return equipments;
        }

        private void SubmitOrderButton(object obj)
        {
            if(Transfer.InventoryItems.Count == 0)
            {
                MessageBox.Show("You didn't input equipments that you want to transfer!");
            }
            else
            {
                if (this.Transfer.TransferType == TransferType.Deferred)
                {
                    this.Transfer.TimeSlot.EndTime = this.DateInput;
                    DelayChangeQuantityEquipment();
                }
                else
                {
                    DelayChangeQuantityEquipment();
                }

                this.MainStorage.Transfers.Add(this.Transfer);
                this.MainStorage.transferStorage.Save(this.MainStorage.Transfers);

                if(this.Transfer.TransferType == TransferType.Instant)
                {
                    this.Transfer.UpdateInventoryItems(this.MainStorage, this.Transfer);
                }

                MessageBox.Show("You just successfully made a transfer!");

                new EquipmentDistributionView(this.MainStorage).Show();
                this.DistributionOrderView.Close();
            }
        }

        public void DelayChangeQuantityEquipment()
        {
            foreach(InventoryItem transferItem in this.Transfer.InventoryItems) 
            {
                foreach (Hospital hospital in this.MainStorage.Hospitals)
                {
                    foreach (Warehouse warehouse in hospital.Warehouses)
                    {
                        if (warehouse.Id == this.Transfer.FromRoomId)
                        {
                            foreach (InventoryItem inventoryItem in warehouse.InventoryItems)
                            {
                                if (inventoryItem.EquipmentId == transferItem.EquipmentId)
                                {
                                    inventoryItem.Quantity = inventoryItem.Quantity - transferItem.Reserved;
                                    inventoryItem.Reserved = transferItem.Reserved;
                                }
                            }
                        }
                    }
                    foreach (Room room in hospital.Rooms)
                    {
                        if (room.Id == this.Transfer.FromRoomId)
                        {
                            foreach (InventoryItem inventoryItem in room.InventoryItems)
                            {
                                if (inventoryItem.EquipmentId == transferItem.EquipmentId)
                                {
                                    inventoryItem.Quantity = inventoryItem.Quantity - transferItem.Reserved;
                                    inventoryItem.Reserved = transferItem.Reserved;
                                }
                            }
                        }
                    }
                }
            }
            
            this.MainStorage.hospitalStorage.Save(this.MainStorage.Hospitals);
        }

        private void BackButton(object obj)
        {
            EquipmentDistributionView equipmentDistributionWindow = new EquipmentDistributionView(this.MainStorage);
            this.DistributionOrderView.Close();
            equipmentDistributionWindow.Show();
        }

    }
}
