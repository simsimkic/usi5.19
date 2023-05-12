using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Converters;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;
using static System.Net.Mime.MediaTypeNames;

namespace ZdravoCorp.ViewModel
{
    public class CrudNurseViewModel : ViewModelBase
    {
        private CrudNurseView crudNurseView { get; set; }
        private MainStorage _mainStorage { get; set; }


        public ICommand CreatePatientAdmissionCommand { get; }
        public ICommand InsertPatientCommand { get; }
        public ICommand EditPatientCommand { get; }
        public ICommand ShowMedicalRecordCommand { get; }
        public ICommand DeletePatientCommand { get; }
        public ICommand BackCommand { get; }

        public Appointment patientsAppointment { get; set; }
        public Patient SelectedPatient { get; set; }
        public List<Patient> PatientsTable { get; set; }

        public CrudNurseViewModel(MainStorage mainStorage, CrudNurseView crudNurseView)
        {
            this._mainStorage = mainStorage;
            this.PatientsTable = mainStorage.Patients;
            this.crudNurseView = crudNurseView;

            InsertPatientCommand = new RelayCommand(InsertPatient);
            EditPatientCommand = new RelayCommand(EditPatient);
            ShowMedicalRecordCommand = new RelayCommand(ShowMedicalRecord);
            DeletePatientCommand = new RelayCommand(DeletePatient);
            CreatePatientAdmissionCommand = new RelayCommand(CreatePatientAdmission);
            BackCommand = new RelayCommand(Back);
        }

        public void InsertPatient(object parameter)
        {
            new InsertPatientView(_mainStorage, crudNurseView).Show();
            this.crudNurseView.Close();
        }

        public void EditPatient(object parameter)
        {
            new EditPatientView(_mainStorage, SelectedPatient, crudNurseView).Show();
            this.crudNurseView.Close();
        }

        public void DeletePatient(object parameter)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // User clicked Yes
                // Perform deletion operation
                _mainStorage.Patients.Remove(SelectedPatient);
                _mainStorage.patientStorage.Save(_mainStorage.Patients);
                CollectionViewSource.GetDefaultView(PatientsTable).Refresh();

            }
        }

        public void ShowMedicalRecord(object parameter)
        {
            new MedicalRecordView(_mainStorage, SelectedPatient.Username, this.crudNurseView).Show();
            this.crudNurseView.Close();
        }

        public void CreatePatientAdmission(object parameter)
        {
            getEarliestPatientAppointment();
            if(patientsAppointment != null)
            {
                new PatientAdmissionView(_mainStorage, SelectedPatient, patientsAppointment).Show();
                this.crudNurseView.Close();
            } 
            else
            {
                MessageBox.Show($"The patient does not have any appointments for the next 15 minutes!");
            }
        }


        public void Back(object parameter)
        {
            new MenuNurseView(_mainStorage).Show();
            this.crudNurseView.Close();

        }

        public void getEarliestPatientAppointment()
        {
            DateTime currentDateTime = DateTime.Now;

            foreach (String appointmentId in SelectedPatient.MedicalRecord.AppointmentIds)
            {
                foreach (Appointment appointment in _mainStorage.Appointments)
                {
                    if (appointmentId == appointment.Id && appointment.TimeSlot.StartTime.Date == currentDateTime.Date)
                    {
                        TimeSpan timeDifference = appointment.TimeSlot.StartTime- currentDateTime;
                        bool isWithin15Minutes = (timeDifference.TotalMinutes) <= 15 && (timeDifference.TotalMinutes) >= 0;
                        if (isWithin15Minutes)
                        {
                            patientsAppointment = appointment;
                        }
                    }
                }
            }
        }


        public Patient FindPatientByUsername(string username)
        {
            Patient p = new Patient();
            Console.WriteLine(username);
            foreach (Patient patient in _mainStorage.Patients)
            {

                if (username == patient.Username)
                {
                    p = patient;
                }
            }

            return p;
        }
    }
}
