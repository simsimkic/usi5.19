using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class UpdateAppointmentViewModel : ViewModelBase
    {


        private MainStorage MainStorage { get; set; }
        private UpdateAppointmentView UpdateAppointmentView { get; set; }

        public Tuple<Appointment, string> AppointmentForUpdate { get; set; }

        public List<Tuple<Appointment, string>> AppointmentForUpdateList { get; set; }


        private string _duration;
        public string Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }


        public DateTime SelectedDateTime { get; set; }
        public ICommand BackCommand { get; }
        public ICommand UpdateCommand { get; }
        public UpdateAppointmentViewModel(MainStorage mainStorage, Tuple<Appointment, string> selectedAppointment, UpdateAppointmentView updateAppointmentView)
        {
            this.MainStorage = mainStorage;
            UpdateAppointmentView = updateAppointmentView;
            this.AppointmentForUpdate = selectedAppointment;

            AppointmentForUpdateList = new List<Tuple<Appointment, string>> { AppointmentForUpdate };

            BackCommand = new RelayCommand(Back);
            UpdateCommand = new RelayCommand(Update);

        }

        public void Update(object parameter)
        {

            Appointment appointment = AppointmentForUpdate.Item1;

            bool isDurationChanged = false;

            if (!int.TryParse(Duration, out int duration))
            {
                MessageBox.Show("End time for appoinment will be same.");
                
            }
            else
            {
                isDurationChanged = true;
            }

            if (SelectedDateTime < DateTime.Now)
            {
                MessageBox.Show("Selected date and time is in the past.");
                return;
            }

            if (isDurationChanged )
            {
                if(appointment.AppointmentType == 0 && duration != 15)
                {
                    MessageBox.Show("Sorry, but duration of appointment must be 15 minutes.");
                    appointment.TimeSlot = new TimeSlot(SelectedDateTime, SelectedDateTime.AddMinutes(15));
                        
                }
                appointment.TimeSlot = new TimeSlot(SelectedDateTime, SelectedDateTime.AddMinutes(duration));
            }
            else
            {
                if(SelectedDateTime > appointment.TimeSlot.EndTime)
                {
                    MessageBox.Show("Not good.");
                    return;
                }
                else
                {
                    if(appointment.AppointmentType == 0 )
                    {
                        MessageBox.Show("Sorry, but duration of appointment must be 15 minutes.");
                        appointment.TimeSlot = new TimeSlot(SelectedDateTime, SelectedDateTime.AddMinutes(15));
                    }
                    appointment.TimeSlot = new TimeSlot(SelectedDateTime, appointment.TimeSlot.EndTime);
                }
                    
            }

            if (!isDoctorAvailable(SelectedDateTime, SelectedDateTime.AddMinutes(duration)))
            {
                MessageBox.Show("You are not available.");
                return;
            }

            if (!isPatientAvailable(SelectedDateTime, SelectedDateTime.AddMinutes(duration), findPatient(AppointmentForUpdate.Item1)))
            {
                MessageBox.Show("Patient is not available.");
                return;
            }

            MainStorage.appointmentStorage.Save(MainStorage.Appointments);
            CollectionViewSource.GetDefaultView(AppointmentForUpdateList).Refresh();

        }

        public void Back(object parameter)
        {
            new DoctorAppointmentsView(this.MainStorage).Show();
            this.UpdateAppointmentView.Close();
        }


        public bool isDoctorAvailable(DateTime startDateTime, DateTime endDateTime)
        {
            Doctor loggedDoctor = new Doctor();
            foreach (Doctor doctor in MainStorage.Doctors)
            {
                if (doctor.Username == this.MainStorage.LoggedPerson.Username)
                {
                    loggedDoctor = doctor;
                }
            }

            List<String> newList = new List<string>(); 
            newList.AddRange(loggedDoctor.AppointmentIds);
            newList.Remove(AppointmentForUpdate.Item1.Id);

            foreach (string appointmentIds in newList)
            {
                Appointment app = findAppointmentById(appointmentIds);
                if (app.AppointmentStatus == AppointmentStatus.Scheduled)
                {
                    if (startDateTime >= app.TimeSlot.StartTime && startDateTime <= app.TimeSlot.EndTime)
                    {
                        return false;
                    }

                    if (endDateTime >= app.TimeSlot.StartTime && endDateTime <= app.TimeSlot.EndTime)
                    {
                        return false;
                    }
                }

            }

            foreach (string free in loggedDoctor.FreeDaysIds)
            {
                FreeDays doctorFree = findDoctorFreeDays(free);
                if (doctorFree != null)
                {
                    if (startDateTime >= doctorFree.TimeSlot.StartTime && startDateTime <= doctorFree.TimeSlot.EndTime)
                    {
                        return false;
                    }
                    if (endDateTime >= doctorFree.TimeSlot.StartTime && endDateTime <= doctorFree.TimeSlot.EndTime)
                    {
                        return false;
                    }

                }
            }

            return true;
        }

        public bool isPatientAvailable(DateTime startDateTime, DateTime endDateTime, Patient selectedPatient)
        {
            //Patient spatient = new Patient();

            foreach (Patient patient in MainStorage.Patients)
            {
                if (selectedPatient.Username == patient.Username)
                {
                    selectedPatient = patient;
                }
            }

            List<String> newList = new List<string>();
            newList.AddRange( selectedPatient.MedicalRecord.AppointmentIds);
            newList.Remove(AppointmentForUpdate.Item1.Id);

            foreach (string appointmentIds in newList)
            {
                Appointment app = findAppointmentById(appointmentIds);
                if (app.AppointmentStatus == AppointmentStatus.Scheduled)
                {
                    if (startDateTime >= app.TimeSlot.StartTime && startDateTime <= app.TimeSlot.EndTime)
                    {
                        return false;
                    }

                    if (endDateTime >= app.TimeSlot.StartTime && endDateTime <= app.TimeSlot.EndTime)
                    {
                        return false;
                    }
                }

            }

            return true;

        }
    
        public Patient findPatient(Appointment appointment)
        {
            foreach(Patient patient in MainStorage.Patients)
            {
                foreach(String ap in patient.MedicalRecord.AppointmentIds)
                {
                    if(appointment.Id == ap)
                    {
                        return patient;
                    }
                }
            }
            return null;
        }
        public FreeDays findDoctorFreeDays(string id)
        {
            FreeDays free = new FreeDays();
            foreach (FreeDays fr in MainStorage.FreeDays)
            {
                if (id == fr.Id)
                {
                    free = fr;
                    break;

                }
            }

            return free;
        }
        public Appointment findAppointmentById(string id)
        {
            Appointment app = new Appointment();
            foreach (Appointment appss in MainStorage.Appointments)
            {
                if (appss.Id == id)
                {
                    app = appss;
                    break;
                }
            }

            return app;
        }

    }
}
