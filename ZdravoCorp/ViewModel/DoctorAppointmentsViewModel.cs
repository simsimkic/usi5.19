using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class DoctorAppointmentsViewModel : ViewModelBase
    {
        public MainStorage MainStorage { get; set; }
        public DoctorAppointmentsView DoctorAppointmentsView { get; set; }

        private ObservableCollection<Tuple<Appointment, string>> _appointmentsWithPatientsUsernameTable;
        public ObservableCollection<Tuple<Appointment, string>> AppointmentsWithPatientsUsernameTable
        {
            get { return _appointmentsWithPatientsUsernameTable; }
            set
            {
                _appointmentsWithPatientsUsernameTable = value;
                OnPropertyChanged(nameof(AppointmentsWithPatientsUsernameTable));
            }
        }

        public Tuple<Appointment, string> SelectedAppointmentWithPatientsUsername { get; set; }

        private DateTime _selectedDate { get; set; }
        public DateTime SelectedDate {
            get { return _selectedDate; }
            set { 
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                PerformUpdate(_selectedDate); 

            }
        }
        
        public ICommand ShowMedicalRecordCommand { get; }
        public ICommand StartExaminationCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }

        public DoctorAppointmentsViewModel(MainStorage mainStorage, DoctorAppointmentsView doctorAppointmentsView)
        {
            this.MainStorage = mainStorage;
            this.DoctorAppointmentsView = doctorAppointmentsView;

            AppointmentsWithPatientsUsernameTable = new ObservableCollection<Tuple<Appointment, string>>(GetAppointmentsWithPatientUsername());
            SelectedDate = DateTime.Now;

            ShowMedicalRecordCommand = new RelayCommand(ShowMedicalRecord);
            StartExaminationCommand = new RelayCommand(StartExamination);

            BackCommand = new RelayCommand(Back);
            CancelCommand = new RelayCommand(Cancel);
            UpdateCommand = new RelayCommand(Update);
            AddCommand = new RelayCommand(Add);

     
        }

        public List<Tuple<Appointment, string>> GetAppointmentsWithPatientUsername()
        {
            Doctor loggedDoctor = MainStorage.Doctors.FirstOrDefault(doctor => doctor.Username == this.MainStorage.LoggedPerson.Username);
            var appointments = MainStorage.Appointments.Where(a => loggedDoctor.AppointmentIds.Contains(a.Id));
            var patients = MainStorage.Patients.SelectMany(p => p.MedicalRecord.AppointmentIds, (p, appointmentId) => new { AppointmentId = appointmentId, p.Username }).ToDictionary(p => p.AppointmentId, p => p.Username);


            var appointmentsWithPatientUsername = from a in appointments
                                                  join p in patients on a.Id equals p.Key
                                                  select Tuple.Create(a, p.Value);

            return appointmentsWithPatientUsername.ToList();
        }

        public void Add(object parameter)
        {
            new CreateAppointmentView(this.MainStorage).Show();
            this.DoctorAppointmentsView.Close();
        }
        public void Update(object parameter)
        {
            if (SelectedAppointmentWithPatientsUsername != null)
            {
                new UpdateAppointmentView(this.MainStorage, SelectedAppointmentWithPatientsUsername).Show();
                this.DoctorAppointmentsView.Close();
            }
            else
            {
                MessageBox.Show("Please select one row.");
            }
        }
        public void Cancel(object parameter)
        {
            Appointment appointment = SelectedAppointmentWithPatientsUsername?.Item1;

            if (appointment != null && appointment.AppointmentStatus == AppointmentStatus.Scheduled)
            {
                appointment.AppointmentStatus = AppointmentStatus.Canceled;
                MainStorage.appointmentStorage.Save(MainStorage.Appointments);
                CollectionViewSource.GetDefaultView(AppointmentsWithPatientsUsernameTable).Refresh();
                MessageBox.Show($"Appointment {appointment.Id} has been canceled.");
            }
            else
            {
                MessageBox.Show("Please select one row with a scheduled appointment.");
            }
        }
        public void Back(object parameter)
        {
            new MenuDoctorView(this.MainStorage).Show();
            this.DoctorAppointmentsView.Close();
        }
       
        public void ShowMedicalRecord(object parameter)
        {

            if (this.SelectedAppointmentWithPatientsUsername != null)
            {
                new MedicalRecordView(this.MainStorage, this.SelectedAppointmentWithPatientsUsername.Item2, this.DoctorAppointmentsView).Show();
                this.DoctorAppointmentsView.Close();
            }
            else
            {
                MessageBox.Show("Please select one row.");
            }
        }

        public void StartExamination(object parameter)
        {
            if (this.SelectedAppointmentWithPatientsUsername != null && this.SelectedAppointmentWithPatientsUsername.Item1.AppointmentStatus == AppointmentStatus.Scheduled)
            {
                var startTime = this.SelectedAppointmentWithPatientsUsername.Item1.TimeSlot.StartTime.AddMinutes(-15); 
                var endTime = this.SelectedAppointmentWithPatientsUsername.Item1.TimeSlot.EndTime;
                var now = DateTime.Now;
                if (now >= startTime && now <= endTime)
                {
                    new DoctorSelectRoomView(this.MainStorage, this.SelectedAppointmentWithPatientsUsername).Show();
                    this.DoctorAppointmentsView.Close();
                }
                else
                {
                    MessageBox.Show($"Selected appointment is not currently available for examination. Please try again between {startTime} and {endTime}.");
                }
            }
            else
            {
                MessageBox.Show("Please select one row and only scheduled appointment.");
            }
        }
        private void PerformUpdate(DateTime selectedDate)
        {
            var threeDaysFromSelectedDate = selectedDate.AddDays(3);

            var filteredAppointments = GetAppointmentsWithPatientUsername().Where(appointment => appointment.Item1.TimeSlot.StartTime >= selectedDate && appointment.Item1.TimeSlot.StartTime < threeDaysFromSelectedDate).ToList();
            AppointmentsWithPatientsUsernameTable = new ObservableCollection<Tuple<Appointment, string>>(filteredAppointments);
            OnPropertyChanged(nameof(AppointmentsWithPatientsUsernameTable));
        }
    }
}
