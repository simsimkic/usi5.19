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
    internal class UpdateMedicalRecordViewModel : ViewModelBase
    {
        public MainStorage MainStorage { get; set; }
        public UpdateMedicalRecordView UpdateMedicalRecordView { get; set; }

        public DoctorPatientListView DoctorPatientListView { get; set; }
        public DoctorExaminationView DoctorExaminationView { get; set; }
        public Patient Patient { get; set; }

        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set
            {
                if (firstName != value)
                {
                    firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                }
            }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set
            {
                if (lastName != value)
                {
                    lastName = value;
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                if (height != value)
                {
                    height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        private int weight;
        public int Weight
        {
            get { return weight; }
            set
            {
                if (weight != value)
                {
                    weight = value;
                    OnPropertyChanged(nameof(Weight));
                }
            }
        }

        private ObservableCollection<string> _medicalHistory = new ObservableCollection<string>();
        public ObservableCollection<string> MedicalHistory
        {
            get { return _medicalHistory; }
            set { _medicalHistory = value; OnPropertyChanged(); }
        }

        private string _newMedicalHistory;
        public string NewMedicalHistory
        {
            get { return _newMedicalHistory; }
            set { _newMedicalHistory = value; OnPropertyChanged(); }
        }

        private string _selectedMedicalHistory;
        public string SelectedMedicalHistory
        {
            get { return _selectedMedicalHistory; }
            set
            {
                if (_selectedMedicalHistory != value)
                {
                    _selectedMedicalHistory = value;
                    OnPropertyChanged(nameof(SelectedMedicalHistory));
                }
            }
        }

        private ObservableCollection<string> _allergies= new ObservableCollection<string>();
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

        public ICommand BackCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand AddNewMedicalHistoryCommand { get; set; }
        public ICommand DeleteMedicalHistoryCommand { get; set; }
        public ICommand AddNewAllergieCommand { get; set; }
        public ICommand DeleteAllergieCommand { get; set; }
        public Tuple<Appointment,string> AppointmentWithPatientsUsername { get; set; }

        public UpdateMedicalRecordViewModel(MainStorage mainStorage, string patientUsername, UpdateMedicalRecordView updateMedicalRecordView)
        {
            this.MainStorage = mainStorage;
            this.UpdateMedicalRecordView = updateMedicalRecordView;
            this.Patient = getPatientByUsername(patientUsername);
            Initialize();
        }

        public UpdateMedicalRecordViewModel(MainStorage mainStorage, Tuple<Appointment, string> appointmentWithPatientsUsername, UpdateMedicalRecordView updateMedicalRecordView, DoctorExaminationView doctorExaminationView)
        {
            this.MainStorage = mainStorage;
            this.UpdateMedicalRecordView = updateMedicalRecordView;
            this.Patient = getPatientByUsername(appointmentWithPatientsUsername.Item2);
            this.DoctorExaminationView = doctorExaminationView;
            this.AppointmentWithPatientsUsername = appointmentWithPatientsUsername;
            Initialize();
        }

        public UpdateMedicalRecordViewModel(MainStorage mainStorage, string patientUsername, UpdateMedicalRecordView updateMedicalRecordView, DoctorPatientListView doctorPatientListView)
        {
            this.MainStorage = mainStorage;
            this.UpdateMedicalRecordView = updateMedicalRecordView;
            this.Patient = getPatientByUsername(patientUsername);
            this.DoctorPatientListView = doctorPatientListView;
            Initialize();
        }

        private void Initialize()
        {
            FirstName = Patient.FirstName;
            LastName = Patient.LastName;
            Height = Patient.MedicalRecord.Height;
            Weight = Patient.MedicalRecord.Weight;
            MedicalHistory = new ObservableCollection<string>(Patient.MedicalRecord.MedicalHistory);
            Allergies = new ObservableCollection<string>(Patient.MedicalRecord.Allergies);

            BackCommand = new RelayCommand(Back);
            EditCommand = new RelayCommand(Edit);
            AddNewMedicalHistoryCommand = new RelayCommand(AddNewMedicalHistory);
            DeleteMedicalHistoryCommand = new RelayCommand(DeleteMedicalHistory);
            AddNewAllergieCommand = new RelayCommand(AddAllergie);
            DeleteAllergieCommand = new RelayCommand(DeleteAllergie);
        }
        public void Back(object parameter)
        {
            if (this.DoctorPatientListView != null)
            {
                new DoctorPatientListView(this.MainStorage).Show();
                this.UpdateMedicalRecordView.Close();
            }

            if (this.DoctorExaminationView != null)
            {
                new DoctorExaminationView(this.MainStorage, this.AppointmentWithPatientsUsername).Show();
                this.UpdateMedicalRecordView.Close();
            }
        }

        public bool ValidateData()
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || Height <= 0 || Weight <= 0)
            {
                return false;
            }
            return true;
        }
        public void Edit(object parameter)
        {
            if (ValidateData())
            {
                this.Patient.FirstName = this.FirstName;
                this.Patient.LastName= this.LastName;
                this.Patient.MedicalRecord.Height = this.Height;
                this.Patient.MedicalRecord.Weight = this.Weight;
                this.Patient.MedicalRecord.MedicalHistory = new List<string>(this.MedicalHistory);
                this.Patient.MedicalRecord.Allergies = new List<string>(this.Allergies);

                this.MainStorage.patientStorage.Save(MainStorage.Patients);

                MessageBox.Show("Successfully changed medical record!");
            }
            else
            {
                MessageBox.Show("Please enter valid data.");
            }
        }
        public Patient getPatientByUsername(string patientUsername)
        {
            return this.MainStorage.Patients.FirstOrDefault(patient => patient.Username == patientUsername);
        }
        public void AddNewMedicalHistory(object parameter)
        {
          
            if (!string.IsNullOrEmpty(NewMedicalHistory))
            {
                MedicalHistory.Add(NewMedicalHistory);
                NewMedicalHistory = string.Empty;
            }
        }
        public void DeleteMedicalHistory(object parameter)
        {
            if (!string.IsNullOrEmpty(SelectedMedicalHistory))
            {
                MedicalHistory.Remove(SelectedMedicalHistory);
                SelectedMedicalHistory = null;
            }
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
                SelectedAllergie = null; 
            }
        }
    }
}
