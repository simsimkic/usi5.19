using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class EquipmentDisplayViewModel : ViewModelBase
    {
        public List<Tuple<Equipment, RoomType, int>> Equipments { get; set; }

        public List<RoomType> RoomTypes { get; set; }

        public List<int> Quantities { get; set; }

        private bool _isOperatingRoomChecked;
        public bool IsOperatingRoomChecked
        {
            get { return _isOperatingRoomChecked; }
            set { _isOperatingRoomChecked = value; OnPropertyChanged(); this.ApplyNewFilter(); }
        }

        private bool _isExaminationRoomChecked;
        public bool IsExaminationRoomChecked
        {
            get { return _isExaminationRoomChecked; }
            set { _isExaminationRoomChecked = value; OnPropertyChanged(); this.ApplyNewFilter(); }
        }

        private bool _isPatientRoomChecked;
        public bool IsPatientRoomChecked
        {
            get { return _isPatientRoomChecked; }
            set { _isPatientRoomChecked = value; OnPropertyChanged(); this.ApplyNewFilter(); }
        }

        private bool _isWaitingRoomChecked;
        public bool IsWaitingRoomChecked
        {
            get { return _isWaitingRoomChecked; }
            set { _isWaitingRoomChecked = value; OnPropertyChanged(); this.ApplyNewFilter(); }
        }

        private bool _isAppointmentsChecked;
        public bool IsAppointmentsChecked
        {
            get { return _isAppointmentsChecked; }
            set { _isAppointmentsChecked = value; OnPropertyChanged(); this.ApplyNewFilter(); }
        }

        private bool _isSurgeriesChecked;
        public bool IsSurgeriesChecked
        {
            get { return _isSurgeriesChecked; }
            set { _isSurgeriesChecked = value; OnPropertyChanged(); this.ApplyNewFilter(); }
        }

        private bool _isRoomFurnitureChecked;
        public bool IsRoomFurnitureChecked
        {
            get { return _isRoomFurnitureChecked; }
            set { _isRoomFurnitureChecked = value; OnPropertyChanged(); this.ApplyNewFilter(); }
        }

        private bool _isHallwayEquipmentsChecked;
        public bool IsHallwayEquipmentsChecked
        {
            get { return _isHallwayEquipmentsChecked; }
            set { _isHallwayEquipmentsChecked = value; OnPropertyChanged(); this.ApplyNewFilter(); }
        }

        private bool _isZeroChecked;
        public bool IsZeroChecked
        {
            get { return _isZeroChecked; }
            set { _isZeroChecked = value; OnPropertyChanged(); this.ApplyNewFilter(); }
        }

        private bool _isLessThan10Checked;
        public bool IsLessThan10Checked
        {
            get { return _isLessThan10Checked; }
            set { _isLessThan10Checked = value; OnPropertyChanged(); this.ApplyNewFilter(); }
        }

        private bool _isMoreThan10Checked;
        public bool IsMoreThan10Checked
        {
            get { return _isMoreThan10Checked; }
            set { _isMoreThan10Checked = value; OnPropertyChanged(); this.ApplyNewFilter();
            }
        }

        private bool _isInsideChecked;
        public bool IsInsideChecked
        {
            get { return _isInsideChecked; }
            set
            {
                _isInsideChecked = value; OnPropertyChanged(); this.ApplyNewFilter();
            }
        }

        private bool _isOutsideChecked;
        public bool IsOutsideChecked
        {
            get { return _isOutsideChecked; }
            set { _isOutsideChecked = value; OnPropertyChanged(); this.ApplyNewFilter();
            }
        }

        public ICommand ResetFilters { get; }
        public ICommand Back { get; }

        public EquipmentDisplayView EquipmentDisplayView { get; set; }
        public MainStorage MainStorage { get; set; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                PerformUpdate(_searchText); // Call your search function here with the updated text
            }
        }

        public EquipmentDisplayViewModel(MainStorage mainStorage, EquipmentDisplayView equipmentDisplayView)
        {
            this.EquipmentDisplayView = equipmentDisplayView;
            this.MainStorage = mainStorage;
            this.ResetFilters = new RelayCommand(ResetFiltersButton);
            this.Back = new RelayCommand(BackButton);

            this.Equipments = new List<Tuple<Equipment, RoomType, int>>();

            for (int i = 0; i < mainStorage.Equipments.Count; i++)
            {
                GetRoomTypeAndQuantityForEquipment(mainStorage.Equipments[i]);
                for (int j = 0; j < this.RoomTypes.Count; j++)
                {
                    Equipments.Add(new Tuple<Equipment, RoomType, int>(mainStorage.Equipments[i], this.RoomTypes[j], this.Quantities[j]));
                }
            }
        }

        private void BackButton(object obj)
        {
            MenuAdministratorView menuWindow = new MenuAdministratorView(this.MainStorage);
            this.EquipmentDisplayView.Close();
            menuWindow.Show();
        }

        public void GetRoomTypeAndQuantityForEquipment(Equipment equipment)
        {
            this.RoomTypes = new List<RoomType>();
            this.Quantities = new List<int>();

            for (int i = 0; i < this.MainStorage.Hospitals.Count; i++)
            {
                foreach (Warehouse warehouse in this.MainStorage.Hospitals[i].Warehouses)
                {
                    foreach (InventoryItem inventoryItem in warehouse.InventoryItems)
                    {
                        if (inventoryItem.EquipmentId.Equals(equipment.Id) == true)
                        {
                            this.RoomTypes.Add((RoomType)4);
                            this.Quantities.Add((int)inventoryItem.Quantity);
                        }
                    }
                }

                foreach (Room room in this.MainStorage.Hospitals[i].Rooms)
                {
                    foreach (InventoryItem inventoryItem in room.InventoryItems)
                    {
                        if (inventoryItem.EquipmentId.Equals(equipment.Id) == true)
                        {
                            this.RoomTypes.Add(room.RoomType);
                            this.Quantities.Add((int)inventoryItem.Quantity);
                        }
                    }
                }
            }
        }
        private void PerformUpdate(string searchText)
        {
            (List<RoomType> typeOfRooms, List<EquipmentType> typeOfEqipments, List<int> quantities, List<RoomType> warehouses) = this.GetSelectedFilters();

            if ((string.IsNullOrWhiteSpace(searchText)) && ((typeOfEqipments.Count + typeOfRooms.Count + quantities.Count + warehouses.Count) == 0))
            {   
                //none either search or filter is in use
                this.Equipments = this.MainStorage.Equipments.SelectMany(equipment =>
                {
                    GetRoomTypeAndQuantityForEquipment(equipment);
                    return this.RoomTypes.Select((roomType, index) => new Tuple<Equipment, RoomType, int>(equipment, roomType, this.Quantities[index]));
                }).ToList();


                OnPropertyChanged(nameof(this.Equipments));

            } else if ((string.IsNullOrWhiteSpace(searchText)))
            {
                //just filter is in use
                this.Equipments = this.MainStorage.Equipments
                    .SelectMany(equipment =>
                    {
                        GetRoomTypeAndQuantityForEquipment(equipment);
                        return this.RoomTypes.Select((roomType, index) => new Tuple<Equipment, RoomType, int>(equipment, roomType, this.Quantities[index]));
                    })
                    .Where(tuple =>
                        (((typeOfRooms.Count != 0 && typeOfRooms.Contains(tuple.Item2)) || (typeOfRooms.Count == 0))
                        && ((typeOfEqipments.Count != 0 && typeOfEqipments.Contains(tuple.Item1.EquipmentType)) || (typeOfEqipments.Count == 0))
                        && ((warehouses.Count != 0 && warehouses.Contains(tuple.Item2)) || (warehouses.Count == 0))
                        && (
                            ((quantities.Count != 0
                            && (((tuple.Item3 == 0) && quantities.Contains(0))
                            || ((tuple.Item3 > 0) && tuple.Item3 <= 10 && quantities.Contains(1))
                            || ((tuple.Item3 > 10) && quantities.Contains(2))))
                            || (quantities.Count == 0)
                        )))
                ).ToList();

                OnPropertyChanged(nameof(this.Equipments));
            }
            else
            {
                //both search and filter is in use
                this.Equipments = this.MainStorage.Equipments
                    .SelectMany(equipment =>
                    {
                        GetRoomTypeAndQuantityForEquipment(equipment);
                        return this.RoomTypes.Select((roomType, index) => new Tuple<Equipment, RoomType, int>(equipment, roomType, this.Quantities[index]));
                    })
                    .Where(tuple =>
                        (tuple.Item1.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                        || tuple.Item1.Id.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                        || tuple.Item1.EquipmentType.ToString().Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                        || tuple.Item2.ToString().Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                        || tuple.Item3.ToString().Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                        && 
                        (((typeOfRooms.Count != 0 && typeOfRooms.Contains(tuple.Item2)) || (typeOfRooms.Count == 0))
                        && ((typeOfEqipments.Count != 0 && typeOfEqipments.Contains(tuple.Item1.EquipmentType)) || (typeOfEqipments.Count == 0))
                        && ((warehouses.Count != 0 && warehouses.Contains(tuple.Item2)) || (warehouses.Count == 0))
                        && (
                            ((quantities.Count != 0
                            && (((tuple.Item3 == 0) && quantities.Contains(0))
                            || ((tuple.Item3 > 0) && tuple.Item3 <= 10 && quantities.Contains(1))
                            || ((tuple.Item3 > 10) && quantities.Contains(2))))
                            || (quantities.Count == 0)
                        )))
                ).ToList();

                OnPropertyChanged(nameof(this.Equipments));

            }
        }

        public void ResetFiltersButton(object parameters)
        {
            this.IsOperatingRoomChecked = false;
            this.IsPatientRoomChecked = false;
            this.IsExaminationRoomChecked = false;
            this.IsWaitingRoomChecked = false;
            this.IsAppointmentsChecked = false;
            this.IsSurgeriesChecked = false;
            this.IsRoomFurnitureChecked = false;
            this.IsHallwayEquipmentsChecked = false;
            this.IsZeroChecked = false;
            this.IsLessThan10Checked = false;
            this.IsMoreThan10Checked = false;
            this.IsInsideChecked = false;
            this.IsOutsideChecked = false;
        }

        public void ApplyNewFilter()
        {
            this.PerformUpdate(this.SearchText);
        }


        public (List<RoomType>, List<EquipmentType>, List<int>, List<RoomType>) GetSelectedFilters()
        {
            List<RoomType> typeOfRooms = new List<RoomType>();
            List<EquipmentType> typeOfEqipments = new List<EquipmentType>();
            List<int> quantities = new List<int>();
            List<RoomType> warehouses = new List<RoomType>();

            if (this.IsOperatingRoomChecked == true)
            {
                typeOfRooms.Add(RoomType.OperatingRoom);
            }
            if (this.IsExaminationRoomChecked == true)
            {
                typeOfRooms.Add(RoomType.ExaminationRoom);
            }
            if (this.IsPatientRoomChecked == true)
            {
                typeOfRooms.Add(RoomType.PatientRoom);
            }
            if (this.IsWaitingRoomChecked == true)
            {
                typeOfRooms.Add(RoomType.WaitingRoom);
            }

            if (this.IsAppointmentsChecked == true)
            {
                typeOfEqipments.Add(EquipmentType.Appointments);
            }
            if (this.IsSurgeriesChecked == true)
            {
                typeOfEqipments.Add(EquipmentType.Surgeries);
            }
            if (this.IsRoomFurnitureChecked == true)
            {
                typeOfEqipments.Add(EquipmentType.RoomFurniture);
            }
            if (this.IsHallwayEquipmentsChecked == true)
            {
                typeOfEqipments.Add(EquipmentType.HallwayEquipments);
            }

            if (this.IsZeroChecked == true)
            {
                quantities.Add(0);
            }
            if (this.IsLessThan10Checked == true)
            {
                quantities.Add(1);
            }
            if (this.IsMoreThan10Checked == true)
            {
                quantities.Add(2);
            }

            if (this.IsOutsideChecked == true)
            {
                warehouses.Add(RoomType.OperatingRoom);
                warehouses.Add(RoomType.ExaminationRoom);
                warehouses.Add(RoomType.PatientRoom);
                warehouses.Add(RoomType.WaitingRoom);
            }
            if (this.IsInsideChecked == true)
            {
                warehouses.Add(RoomType.WareHouse);
            }

            return (typeOfRooms, typeOfEqipments, quantities, warehouses);
        }

    }
}
