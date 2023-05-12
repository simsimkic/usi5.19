using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class DoctorEquipmentUsageViewModel : ViewModelBase
    {
        public MainStorage MainStorage { get; set; }
        public Appointment Appointment { get; set; }
        public DoctorEquipmentUsageView  DoctorEquipmentUsage { get; set; }
       
        private ObservableCollection<Tuple<Equipment, int>> _equipmentsWithQuantityTable;
        public ObservableCollection<Tuple<Equipment, int>> EquipmentsWithQuantityTable
        {
            get { return _equipmentsWithQuantityTable; }
            set
            {
                _equipmentsWithQuantityTable = value;
                OnPropertyChanged(nameof(EquipmentsWithQuantityTable));
            }
        }

        private Tuple<Equipment, int> _selectedEquipmentWithQuantity;
        public Tuple<Equipment, int> SelectedEquipmentWithQuantity
        {
            get { return _selectedEquipmentWithQuantity; }
            set
            {
                _selectedEquipmentWithQuantity = value;
                OnPropertyChanged(nameof(SelectedEquipmentWithQuantity));

                TakenQuantity = SelectedEquipmentWithQuantity == null ? 0 : SelectedEquipmentWithQuantity.Item2;
            }
        }

        private int _takenQuantity;
        public int TakenQuantity
        {
            get { return _takenQuantity; }
            set { SetProperty(ref _takenQuantity, value); }
        }

        public ICommand ReduceQuantityCommand { get; }
        public ICommand FinishCommand { get; }

        public DoctorEquipmentUsageViewModel(MainStorage mainStorage, Appointment appointment,DoctorEquipmentUsageView doctorEquipmentUsageView)
        {
            this.MainStorage = mainStorage;
            this.Appointment = appointment;
            this.DoctorEquipmentUsage = doctorEquipmentUsageView;

            this.EquipmentsWithQuantityTable = new ObservableCollection<Tuple<Equipment, int>>(GetDynamicEquipment(appointment.RoomId));

            ReduceQuantityCommand = new RelayCommand(ReduceQuantity);
            FinishCommand = new RelayCommand(Finish);
            
        }

        public void Finish(object parameter)
        {
            new DoctorAppointmentsView(this.MainStorage).Show();
            this.DoctorEquipmentUsage.Close();
        }
        public List<Tuple<Equipment, int>> GetDynamicEquipment(string roomId)
        {
            var room = MainStorage.Hospitals[0].Rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
            {
                return new List<Tuple<Equipment, int>>();
            }

            var dynamicEquipments = room.InventoryItems
                .Select(inventoryItem =>
                {
                    var equipment = MainStorage.Equipments.FirstOrDefault(e => e.Id == inventoryItem.EquipmentId);
                    return new { Equipment = equipment, Quantity = inventoryItem.Quantity };
                })
                .Where(x => x.Equipment != null && (x.Equipment.EquipmentType == EquipmentType.Appointments || x.Equipment.EquipmentType == EquipmentType.Surgeries))
                .Select(x => new Tuple<Equipment, int>(x.Equipment, x.Quantity))
                .ToList();

            return dynamicEquipments;
        }
        public void ReduceQuantity(object parameter)
        {
            int quantityToReduce = TakenQuantity;
            Tuple<Equipment, int> selectedEquipment = SelectedEquipmentWithQuantity;

            if (selectedEquipment != null)
            {
                if (selectedEquipment.Item2 >= quantityToReduce)
                {
                    int newQuantity = selectedEquipment.Item2 - quantityToReduce;
                    SelectedEquipmentWithQuantity = Tuple.Create(selectedEquipment.Item1, newQuantity);
                    EquipmentsWithQuantityTable = new ObservableCollection<Tuple<Equipment, int>>(
                        EquipmentsWithQuantityTable.Select(e =>
                            e.Item1.Id == selectedEquipment.Item1.Id ? Tuple.Create(selectedEquipment.Item1, newQuantity) : e
                        )
                    );

                    Room room = MainStorage.Hospitals[0].Rooms.Find(r => r.Id == Appointment.RoomId);
                    if (room != null)
                    {
                        InventoryItem inventoryItem = room.InventoryItems.Find(i => i.EquipmentId == selectedEquipment.Item1.Id);
                        if (inventoryItem != null)
                        {
                            inventoryItem.Quantity = newQuantity;
                            MainStorage.hospitalStorage.Save(MainStorage.Hospitals);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not enough quantity available to reduce.");
                }
            }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }



    }
}
