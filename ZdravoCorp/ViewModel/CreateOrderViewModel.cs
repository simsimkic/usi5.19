using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.View;
using ZdravoCorp.Storage;
using ZdravoCorp.Model;
using System.Windows.Input;
using ZdravoCorp.Commands;
using System.Windows;
using System.Timers;
using System.Windows.Media.TextFormatting;
using System.Windows.Controls;

namespace ZdravoCorp.ViewModel
{
    public class CreateOrderViewModel : ViewModelBase
    {
        public Order Order { get; set; }    
        public string RoomId { get; set; }
        public List<Tuple<Equipment, int>> Equipments { get; set; }
        public int? QuantityInput { get; set; }
        public int DateInput { get; set; }
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

        public CreateOrderView CreateOrderView { get; set; }
        public CreateOrderViewModel(MainStorage mainStorage, CreateOrderView createOrderView)
        {
            this.MainStorage = mainStorage;
            this.CreateOrderView = createOrderView;
            this.Order = new Order(this.MainStorage);

            this.SaveOrder = new RelayCommand(SaveOrderButton);
            this.SubmitOrder = new RelayCommand(SubmitOrderButton);
            this.Back = new RelayCommand(BackButton);

            this.Equipments = GetDynamicEquipments();

        }

        private void SaveOrderButton(object obj)
        {
            if (this.QuantityInput <= 0)
            {
                MessageBox.Show("You can't order less than 1 equipment!");
            }
            else
            {
                OrderItem orderItem = new OrderItem(SelectedRow.Item1, (int)QuantityInput);
                this.Order.OrderItems.Add(orderItem);

                MessageBox.Show($"You add {orderItem.Equipment.Name} to Order!");
            }
        }

        public List<Tuple<Equipment, int>> GetDynamicEquipments()
        {
            List < Tuple<Equipment, int>> equipments = new List <Tuple<Equipment, int>>();

            foreach (Equipment equipment in this.MainStorage.Equipments) {
                int quantity = 0;
            
                for (int i = 0; i < this.MainStorage.Hospitals.Count; i++){
                    foreach (Warehouse warehouse in this.MainStorage.Hospitals[i].Warehouses)
                    {
                        foreach (InventoryItem inventoryItem in warehouse.InventoryItems)
                        {
                                if (inventoryItem.EquipmentId.Equals(equipment.Id) == true)
                                {
                                    quantity = quantity + inventoryItem.Quantity;
                                }
                        }
                    }

                    foreach (Room room in this.MainStorage.Hospitals[i].Rooms)
                    {
                        foreach (InventoryItem inventoryItem in room.InventoryItems)
                        {
                                if (inventoryItem.EquipmentId.Equals(equipment.Id) == true)
                                {
                                    quantity = quantity + inventoryItem.Quantity;
                                }
                            }
                    }
                }

                if((quantity < 5) && ( equipment.EquipmentType == EquipmentType.Surgeries || equipment.EquipmentType == EquipmentType.Appointments))
                {
                    equipments.Add(new Tuple<Equipment, int>(equipment, quantity));
                }
            }

            return equipments;
        }

        
        private void SubmitOrderButton(object obj)
        {
            MainStorage.Orders.Add(this.Order);
            MainStorage.orderStorage.Save(MainStorage.Orders);
                
            MessageBox.Show("You just successfully made an order!");

            MenuAdministratorView menuWindow = new MenuAdministratorView(this.MainStorage);
            this.CreateOrderView.Close();
            menuWindow.Show();
        }
        private void BackButton(object obj)
        {
            MenuAdministratorView menuWindow = new MenuAdministratorView(this.MainStorage);
            this.CreateOrderView.Close();
            menuWindow.Show();
        }

    }
}
