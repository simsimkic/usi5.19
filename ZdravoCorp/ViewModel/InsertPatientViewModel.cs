using System;
using System.Collections.Generic;
using System.Linq;
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
    public class InsertPatientViewModel : ViewModelBase
    {
        private MainStorage _mainStorage { get; set; }
        private CrudNurseView crudNurseView { get; set; }
        private InsertPatientView insertPatientView { get; set; }
        public ICommand InsertPatientCommand { get; }
        public ICommand BackCommand { get; }
        private Patient newPatient { get; set; }

        

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

        public string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }




        public InsertPatientViewModel(MainStorage mainStorage, CrudNurseView crudNurseView, InsertPatientView insertPatientView)
        {
            _mainStorage = mainStorage;
            this.crudNurseView = crudNurseView;
            this.insertPatientView = insertPatientView;

            InsertPatientCommand = new RelayCommand(InsertPatient);
            BackCommand = new RelayCommand(Back);
        }

        public void InsertPatient(object paramter)
        {
            
            bool isUnique = isUsernameUnique(_username);
            bool isValidInput = isInputForPatientEmpty(_firstName, _lastName, _username, _password);

            if (isUnique && isValidInput)
            {
                Person newPerson = new Person(_firstName, _lastName, _username, _password, Status.Active);
                MedicalRecord medicalRecord = new MedicalRecord();
                newPatient = new Patient(newPerson, medicalRecord);
                

                new InsertMedicalRecordView(_mainStorage, newPatient, crudNurseView).Show();
                this.insertPatientView.Close();

            }
            else if (!isUnique)
            {
                MessageBox.Show($"Check again! Usename ALREADY EXISTS!");
            }
            else if (!isValidInput)
            {
                MessageBox.Show($"Check again! Some input fields are EMPTY!");
            }
        }


        public bool isUsernameUnique(string username)
        {
            bool isUnique = true;
            foreach (Patient patient in _mainStorage.Patients)
            {
                if (username == patient.Username)
                {
                    isUnique = false;
                }
            }

            return isUnique;
        }


        public bool isInputForPatientEmpty(string firstName, string lastName, string username, string password)
        {

            Console.WriteLine(password);
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            else if (!char.IsUpper(firstName[0]) || !char.IsUpper(lastName[0]))
            {
                MessageBox.Show($"Check again! First and last name must start with a capital letter!");
                return false;
            }

            return true;
        }

        public void Back(object parameter)
        {
            new CrudNurseView(this._mainStorage).Show();
            this.insertPatientView.Close();
        }

    }
}
