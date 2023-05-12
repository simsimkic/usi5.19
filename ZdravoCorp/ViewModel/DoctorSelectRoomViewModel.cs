using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class DoctorSelectRoomViewModel :  ViewModelBase
    {
        public MainStorage MainStorage { get; set; }
        public DoctorSelectRoomView DoctorSelectRoomView { get; set; }
        public Tuple<Appointment, string> AppointmenWithPatientsUsername { get; set; }

        private Room _selectedRoom { get; set; }
        public Room SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                _selectedRoom = value;
                OnPropertyChanged(nameof(SelectedRoom));
            }
        }

        private ObservableCollection<Room> _roomsTable;
        public ObservableCollection<Room> RoomsTable
        {
            get { return _roomsTable; }
            set
            {
                _roomsTable = value;
                OnPropertyChanged(nameof(RoomsTable));
            }
        }
        public ICommand BackCommand { get; }
        public ICommand SelectRoomCommand { get; }
        public DoctorSelectRoomViewModel(MainStorage mainStorage, Tuple<Appointment, string> appointmentWithPatientsUsername, DoctorSelectRoomView doctorSelectRoomView)
        {
            this.MainStorage = mainStorage;
            this.DoctorSelectRoomView = doctorSelectRoomView;
            this.AppointmenWithPatientsUsername = appointmentWithPatientsUsername;
            this.RoomsTable = new ObservableCollection<Room>(mainStorage.Hospitals[0].Rooms);

            BackCommand = new RelayCommand(Back);
            SelectRoomCommand = new RelayCommand(SelectRoom);

        }
        public void SelectRoom(object parameter)
        {
            Appointment appointment = AppointmenWithPatientsUsername.Item1;
            if (SelectedRoom != null && isRoomAvailable(SelectedRoom, appointment.TimeSlot))
            {
                appointment.RoomId = SelectedRoom.Id;
                this.MainStorage.appointmentStorage.Save(MainStorage.Appointments);
                MessageBox.Show($"Room {SelectedRoom.Id} has been successfully assigned to the appointment {appointment.Id}.");

                new DoctorExaminationView(this.MainStorage, this.AppointmenWithPatientsUsername).Show();
                this.DoctorSelectRoomView.Close();
            }
            else
            {
                MessageBox.Show(SelectedRoom == null ? "Please select a row." : $"Room {SelectedRoom.Id} is currently not available. Please select a different room.");
            }
        }
        public bool isRoomAvailable(Room room, TimeSlot timeSlot)
        {
            return !MainStorage.Appointments.Any(appointment =>
                appointment.RoomId == room.Id && appointment.TimeSlot.StartTime <= timeSlot.EndTime && appointment.TimeSlot.EndTime >= timeSlot.EndTime);
        }
        public void Back(object parameter)
        {
            new DoctorAppointmentsView(this.MainStorage).Show();
            this.DoctorSelectRoomView.Close();
        }
    }
}
