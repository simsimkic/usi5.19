using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class PatientAdmissionViewModel : ViewModelBase
    {

        private MainStorage mainStorage { get; set; }
        private Patient selectedPatient { get; set; }
        private PatientAdmissionView PatientAdmissionView { get; set; }
        public Appointment patientsAppointment { get; set; }

        public ICommand BackCommand { get; }
        public ICommand CreatePatientAdmissionCommand { get; }


        private ObservableCollection<string> _allergies = new ObservableCollection<string>();
        public ObservableCollection<string> Allergies
        {
            get { return _allergies; }
            set { _allergies = value; OnPropertyChanged(); }
        }
        private string _newAllergie;
        public string NewAllergie
        {
            get { return _newAllergie; }
            set { _newAllergie = value; OnPropertyChanged(); }
        }

        private string _selectedAllergie;
        public string SelectedAllergie
        {
            get { return _selectedAllergie; }
            set
            {
                if (_selectedAllergie != value)
                {
                    _selectedAllergie = value;
                    OnPropertyChanged(nameof(SelectedAllergie));
                }
            }
        }


        private ObservableCollection<string> _medicalDiseases = new ObservableCollection<string>();
        public ObservableCollection<string> MedicalHistory
        {
            get { return _medicalDiseases; }
            set { _medicalDiseases = value; OnPropertyChanged(); }
        }
        private string _newMedicalDisease;
        public string NewMedicalDisease
        {
            get { return _newMedicalDisease; }
            set { _newMedicalDisease = value; OnPropertyChanged(); }
        }

        private string _selectedMedicalDisease;
        public string SelectedMedicalDisease
        {
            get { return _selectedMedicalDisease; }
            set
            {
                if (_selectedMedicalDisease != value)
                {
                    _selectedMedicalDisease = value;
                    OnPropertyChanged(nameof(SelectedMedicalDisease));
                }
            }
        }

        private ObservableCollection<string> _symptoms = new ObservableCollection<string>();
        public ObservableCollection<string> Symptoms
        {
            get { return _symptoms; }
            set { _symptoms = value; OnPropertyChanged(); }
        }
        private string _newSymptom;
        public string NewSymptom
        {
            get { return _newSymptom; }
            set { _newSymptom = value; OnPropertyChanged(); }
        }

        private string _selectedSymptom;
        public string SelectedSymptom
        {
            get { return _selectedSymptom; }
            set
            {
                if (_selectedSymptom != value)
                {
                    _selectedSymptom = value;
                    OnPropertyChanged(nameof(SelectedSymptom));
                }
            }
        }

        public ICommand AddNewAllergieCommand { get; }
        public ICommand DeleteAllergieCommand { get; }

        public ICommand AddNewMedicalDiseaseCommand { get; }
        public ICommand DeleteMedicalDiseaseCommand { get; }

        public ICommand AddNewSymptomCommand { get; }
        public ICommand DeleteSymptomCommand { get; }

        public PatientAdmissionViewModel(MainStorage mainStorage, Patient selectedPatient, Appointment patientsAppointment, PatientAdmissionView patientAdmissionView)
        {
            this.mainStorage = mainStorage;
            this.selectedPatient = selectedPatient;
            this.patientsAppointment = patientsAppointment;
            PatientAdmissionView = patientAdmissionView;

            Allergies = new ObservableCollection<string>(selectedPatient.MedicalRecord.Allergies);
            MedicalHistory = new ObservableCollection<string>(selectedPatient.MedicalRecord.MedicalHistory);

            setBindings();

            CreatePatientAdmissionCommand = new RelayCommand(CreateAdmission);

            AddNewAllergieCommand = new RelayCommand(AddAllergie);
            DeleteAllergieCommand = new RelayCommand(DeleteAllergie);

            AddNewMedicalDiseaseCommand = new RelayCommand(AddMedicalDisease);
            DeleteMedicalDiseaseCommand = new RelayCommand(DeleteMedicalDisease);

            AddNewSymptomCommand = new RelayCommand(AddSymptom);
            DeleteSymptomCommand = new RelayCommand(DeleteSymptom);


            BackCommand = new RelayCommand(Back);
        }

        public void setBindings()
        {
            PatientAdmissionView.UsernameTextBox.SetBinding(TextBox.TextProperty, new Binding("Username") { Source = selectedPatient });
            PatientAdmissionView.AppointmentsIDTextBox.SetBinding(TextBox.TextProperty, new Binding("Id") { Source = patientsAppointment});
        }

        public void CreateAdmission(object parameter)
        {

            selectedPatient.MedicalRecord.MedicalHistory = MedicalHistory.ToList();
            selectedPatient.MedicalRecord.Allergies = Allergies.ToList();
            foreach(String s in Symptoms)
            {
                Console.WriteLine(s);
            }
            patientsAppointment.Anamnesis.Symptoms = Symptoms.ToList();

            mainStorage.patientStorage.Save(mainStorage.Patients);
            mainStorage.appointmentStorage.Save(mainStorage.Appointments);

            MessageBox.Show($"Successful patient admission!" + selectedPatient.FirstName + " " + selectedPatient.LastName);

            new CrudNurseView(mainStorage).Show();
            this.PatientAdmissionView.Close();


        }

        public void AddAllergie(object parameter)
        {

            if (!string.IsNullOrEmpty(NewAllergie))
            {
                Allergies.Add(NewAllergie);
                NewAllergie = string.Empty;
            }
        }

        public void DeleteAllergie(object parameter)
        {
            if (!string.IsNullOrEmpty(SelectedAllergie))
            {
                Allergies.Remove(SelectedAllergie);
                SelectedAllergie= null;
            }

        }

        public void AddMedicalDisease(object parameter)
        {

            if (!string.IsNullOrEmpty(NewMedicalDisease))
            {
                MedicalHistory.Add(NewMedicalDisease);
                NewMedicalDisease = string.Empty;
            }
        }

        public void DeleteMedicalDisease(object parameter)
        {
            if (!string.IsNullOrEmpty(SelectedMedicalDisease))
            {
                MedicalHistory.Remove(SelectedMedicalDisease);
                SelectedMedicalDisease= null;
            }

        }

        public void AddSymptom(object parameter)
        {
            if (!string.IsNullOrEmpty(NewSymptom))
            {
                Symptoms.Add(NewSymptom);
                NewSymptom = string.Empty;
            }

        }

        public void DeleteSymptom(object parameter)
        {
            if (!string.IsNullOrEmpty(SelectedSymptom))
            {
                Symptoms.Remove(SelectedSymptom);
                SelectedSymptom = null;
            }
        }

        public void Back(object parameter)
        {
            new CrudNurseView(this.mainStorage).Show();
            this.PatientAdmissionView.Close();
        }


    }
}
