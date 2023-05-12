using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class EquipmentDistributionViewModel : ViewModelBase
    {
        MainStorage MainStorage { get; set; }
        EquipmentDistributionView EquipmentDistributionView { get; set; }

        private List<string> _fromRoom;
        public List<string> FromRoom
        {
            get { return _fromRoom; }
            set
            {
                _fromRoom = value;
            }
        }

        private string _selectedFromRoom;
        public string SelectedFromRoom
        {
            get { return _selectedFromRoom; }
            set { _selectedFromRoom = value; }
        }

        private List<string> _toRoom;
        public List<string> ToRoom
        {
            get { return _toRoom; }
            set
            {
                _toRoom = value;
            }
        }

        private string _selectedToRoom;
        public string SelectedToRoom
        {
            get { return _selectedToRoom; }
            set { _selectedToRoom = value; }
        }

        private bool _isDyinamicChecked;
        public bool IsDyinamicChecked
        {
            get { return _isDyinamicChecked; }
            set { _isDyinamicChecked = value; OnPropertyChanged(); }
        }

        public string UrgentRooms { get; set; }
        public ICommand SubmitButton { get; }
        public ICommand BackButton { get; }
        public EquipmentDistributionViewModel() { }
        public EquipmentDistributionViewModel(MainStorage mainStorage, EquipmentDistributionView equipmentDistributionView)
        {
            this.MainStorage = mainStorage;
            this.EquipmentDistributionView = equipmentDistributionView;
            this.FromRoom = getExitingRooms();
            this.ToRoom = getExitingRooms();
            this.UrgentRooms = isLessThanFiveEquipmentsByRoom();
            this.SubmitButton = new RelayCommand(submitButton);
            this.BackButton = new RelayCommand(backButton);

        }

        public string isLessThanFiveEquipmentsByRoom()
        {
            string description = ("Rooms with less than 5 Equipments Quantity:\n").ToUpper();
            string equipmentsByRoom = "";
            foreach (Hospital hospital in this.MainStorage.Hospitals)
            {
                foreach (Warehouse warehouse in hospital.Warehouses)
                {
                    foreach (InventoryItem item in warehouse.InventoryItems)
                    {
                        if (item.Quantity < 5)
                        {
                            if(equipmentsByRoom.Contains(warehouse.Id) != true)
                            {
                                equipmentsByRoom = equipmentsByRoom + "|" + warehouse.Id;
                            }
                        }
                    }
                }
                foreach (Room room in hospital.Rooms)
                {
                    foreach (InventoryItem item in room.InventoryItems)
                    {
                        if (item.Quantity < 5)
                        {
                            if (equipmentsByRoom.Contains(room.Id) != true)
                            {
                                equipmentsByRoom = equipmentsByRoom + "|" + room.Id;
                            }
                        }
                    }
                }
            }

            if(equipmentsByRoom == "")
            {
                equipmentsByRoom = ("There is no room with less than 5 equipment quantity!").ToUpper();
            }
            else
            {
                equipmentsByRoom = description + equipmentsByRoom +"|";
            }

            return equipmentsByRoom;
        }

        public void submitButton(object obj)
        {
            string fromRoom = this.SelectedFromRoom;
            string toRoom = this.SelectedToRoom;
            bool isDynamicEqipment = IsDyinamicChecked;
            if( fromRoom == toRoom )
            {
                MessageBox.Show("You can't distribute eqipments into the same room!");
            }
            else
            {
                if(isDynamicEqipment == false)
                {
                    Transfer transfer = new Transfer(this.MainStorage, fromRoom, toRoom, TransferType.Deferred);
                    new DistributionOrderView(this.MainStorage, transfer).Show();
                    this.EquipmentDistributionView.Close();
                }
                else
                {
                    Transfer transfer = new Transfer(this.MainStorage, fromRoom, toRoom, TransferType.Instant);
                    new DistributionOrderView(this.MainStorage, transfer).Show();
                    this.EquipmentDistributionView.Close();
                }
            }
        }
        public void backButton(object obj)
        {
            new MenuAdministratorView(this.MainStorage).Show();
            this.EquipmentDistributionView.Close();
        }

        public void InitializeComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.ItemsSource = this.FromRoom;
        }

        public List<string> getExitingRooms()
        {
            List<string> rooms = new List<string>();
            foreach (Hospital hospital in this.MainStorage.Hospitals){
                foreach (Warehouse warehouse in hospital.Warehouses)
                {
                    rooms.Add(warehouse.Id);
                }
                foreach (Room room in hospital.Rooms)
                {
                    rooms.Add(room.Id);
                }
            }
            return rooms;
        }
    }
}
