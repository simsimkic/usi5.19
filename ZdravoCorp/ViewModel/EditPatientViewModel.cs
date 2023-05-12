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
    public class EditPatientViewModel :ViewModelBase
    {
        private MainStorage mainStorage { get; set; }
        private CrudNurseView crudNurseView { get; set; }
        private EditPatientView editPatientView { get; set; }
        public ICommand EditPatientCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand AddNewMedicalDiseaseCommand { get; }
        public ICommand DeleteMedicalDiseaseCommand { get; }
        public Patient Patient { get; set; }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _weight;
        public string Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(_weight));
            }
        }

        private string _height;
        public string Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
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

        private bool _isActiveStatusSelected;

        public bool IsActiveStatusSelected
        {
            get { return _isActiveStatusSelected; }
            set
            {
                _isActiveStatusSelected = value;
            }
        }

        private bool _isBlockedStatusSelected;

        public bool IsBlockedStatusSelected
        {
            get { return _isBlockedStatusSelected; }
            set
            {
                _isBlockedStatusSelected = value;
            }
        }
        


        public EditPatientViewModel(MainStorage mainStorage, Patient selectedPatient, CrudNurseView crudNurseView, EditPatientView editPatientView)
        {
            this.mainStorage = mainStorage;
            this.crudNurseView = crudNurseView;
            this.editPatientView = editPatientView;
            this.Patient = getPatientByUsername(selectedPatient.Username);

            MedicalHistory = new ObservableCollection<string>(Patient.MedicalRecord.MedicalHistory);
            FirstName = Patient.FirstName;
            LastName = Patient.LastName;
            Username = Patient.Username;
            Password = Patient.Password;
            Weight = Patient.MedicalRecord.Weight.ToString();
            Height = Patient.MedicalRecord.Height.ToString();
            if(Patient.Status == 0)
            {
                IsActiveStatusSelected = true;
            }
            else
            {
                IsBlockedStatusSelected = false;
            }

            EditPatientCommand = new RelayCommand(EditPatient);
            BackCommand = new RelayCommand(Back);
            AddNewMedicalDiseaseCommand = new RelayCommand(AddNewMedicalDisease);
            DeleteMedicalDiseaseCommand = new RelayCommand(DeleteMedicalDisease);

            //SetBindings();
        }

        public void AddNewMedicalDisease(object parameter)
        {

            if (!string.IsNullOrEmpty(NewMedicalHistory))
            {
                MedicalHistory.Add(NewMedicalHistory);
                NewMedicalHistory = string.Empty;
            }
        }

        public void DeleteMedicalDisease(object parameter)
        {
            if (!string.IsNullOrEmpty(SelectedMedicalHistory))
            {
                MedicalHistory.Remove(SelectedMedicalHistory);
                SelectedMedicalHistory = null;
            }
        }

        public Patient getPatientByUsername(string patientUsername)
        {
            foreach (Patient patient in mainStorage.Patients)
            {
                if (patient.Username == patientUsername)
                {
                    return patient;
                }
            }

            return null;
        }

        

        public void EditPatient(object parameter)
        {
            bool isValidInput = isInputForEditEmpty(_firstName, _lastName, _username, _password, _height, _weight);

            if (isValidInput)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to edit this patient?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {

                    Patient patient = FindPatientByUsername(_username);
                    patient.FirstName = _firstName;
                    patient.LastName =_lastName;
                    patient.Username = _username;
                    patient.Password = _password;
                    if (_isActiveStatusSelected)
                    {
                        patient.Status = Status.Active;
                    }
                    else
                    {
                        patient.Status = Status.Blocked;
                    }
                    patient.MedicalRecord.Height = int.Parse(_height);
                    patient.MedicalRecord.Weight = int.Parse(_weight);
                    patient.MedicalRecord.MedicalHistory = MedicalHistory.ToList();

                    mainStorage.patientStorage.Save(mainStorage.Patients);
                    new CrudNurseView(mainStorage).Show();
                    this.editPatientView.Close();

                }
            }
        }

        public bool isInputForEditEmpty(string firstName, string lastName, string username, string password, string height, string weight)
        {
            int number;
            bool isHeightInteger = int.TryParse(height, out number);
            bool isWeightInteger = int.TryParse(weight, out number);
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(height) || string.IsNullOrEmpty(weight))
            {
                MessageBox.Show($"Check again! Some input fields are EMPTY!");
                return false;
            }
            else if (!char.IsUpper(firstName[0]) || !char.IsUpper(lastName[0]))
            {
                MessageBox.Show($"Check again! First and last name must start with a capital letter!");
                return false;
            }
            else if (!isHeightInteger || !isWeightInteger)
            {
                MessageBox.Show($"Check again! Height or weight is WRONG!");
                return false;
            }


            return true;
        }

        public Patient FindPatientByUsername(string username)
        {
            Patient p = new Patient();
            Console.WriteLine(username);
            foreach (Patient patient in mainStorage.Patients)
            {

                if (username == patient.Username)
                {
                    p = patient;
                }
            }

            return p;
        }

        public void Back(object parameter)
        {
            new CrudNurseView(mainStorage).Show();
            this.editPatientView.Close();
        }
    }
}
