using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZdravoCorp.Commands;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class MenuAdministratorViewModel : ViewModelBase
    {
        MainStorage MainStorage { get; set; }
        MenuAdministratorView MenuAdministratorView { get; set; }
        public ICommand DisplayEquipment { get; }
        public ICommand OrderEquipment { get; }
        public ICommand EquipmentDistribution { get; }
        public ICommand BackButton { get; }
        public MenuAdministratorViewModel() { }
        public MenuAdministratorViewModel(MainStorage mainStorage, MenuAdministratorView menuAdministratorView) { 
        
            this.MainStorage = mainStorage;
            this.MenuAdministratorView = menuAdministratorView;
            this.DisplayEquipment = new RelayCommand(displayEquipmentView);
            this.OrderEquipment = new RelayCommand(orderEquipmentView);
            this.EquipmentDistribution = new RelayCommand(equipmentDistributionView);
            this.BackButton = new RelayCommand(Back);
        }

        private void Back(object obj)
        {
            new LogInView().Show();
            this.MenuAdministratorView.Close();
        }

        private void equipmentDistributionView(object obj)
        {
            EquipmentDistributionView equipmentDistributionView = new EquipmentDistributionView(this.MainStorage);
            equipmentDistributionView.Show();
            this.MenuAdministratorView.Close();
        }

        private void orderEquipmentView(object obj)
        {
            CreateOrderView orderView = new CreateOrderView(this.MainStorage);
            orderView.Show();
            this.MenuAdministratorView.Close();
        }

        private void displayEquipmentView(object obj)
        {
            EquipmentDisplayView equipmentDisplayView = new EquipmentDisplayView(this.MainStorage);
            equipmentDisplayView.Show();
            this.MenuAdministratorView.Close();
        }
    }
}
