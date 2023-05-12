using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class InsertMedicalRecordViewModel : ViewModelBase
    {
        private MainStorage mainStorage { get; set; }
        private CrudNurseView crudNurseView { get; set; }
        private InsertMedicalRecordView insertMedicalRecordView { get; set; }
        private Patient newPatient { get; }

        public ICommand InsertMedicalRecordCommand { get; set; }
        public ICommand BackCommand { get; }

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

        private string _medicalHistory;
        public string MedicalHistory
        {
            get { return _medicalHistory; }
            set
            {
                _medicalHistory = value;
                OnPropertyChanged(nameof(MedicalHistory));
            }
        }

        public InsertMedicalRecordViewModel(MainStorage mainStorage, CrudNurseView crudNurseView, Patient newPatient, InsertMedicalRecordView insertMedicalRecordView)
        {
            this.mainStorage = mainStorage;
            this.crudNurseView = crudNurseView;
            this.newPatient = newPatient;
            this.insertMedicalRecordView = insertMedicalRecordView;

            InsertMedicalRecordCommand = new RelayCommand(InsertMedicalRecord);
            BackCommand = new RelayCommand(Back);
        }
            
        public void InsertMedicalRecord(object parameter)
        {
            bool isValid = isMedicalRecordValid(_height, _weight);
            bool isValidInput = isInputForMedicalRecordEmpty(_height, _weight, _medicalHistory);

            if (isValid && isValidInput)
            {
                string[] medicalHistoryArray = _medicalHistory.Split(',');
                List<string> medicalHistoryList = medicalHistoryArray.ToList();

                newPatient.MedicalRecord.Height = int.Parse(_height);
                newPatient.MedicalRecord.Weight = int.Parse(_weight);
                newPatient.MedicalRecord.MedicalHistory = medicalHistoryList;


                mainStorage.Patients.Add(newPatient);
                mainStorage.patientStorage.Save(mainStorage.Patients);

                new CrudNurseView(mainStorage).Show();
                insertMedicalRecordView.Close();

            }
            else if (!isValidInput)
            {
                MessageBox.Show($"Check again! Some input fields are EMPTY!");
            }
            else
            {
                MessageBox.Show($"Check again! Height or weight is WRONG!");
            }
        }

        public bool isMedicalRecordValid(string height, string weight)
        {
            int number;
            bool isHeightInteger = int.TryParse(height, out number);
            bool isWeightInteger = int.TryParse(weight, out number);
            if (!isHeightInteger || !isWeightInteger)
            {
                return false;
            }

            return true;
        }

        public bool isInputForMedicalRecordEmpty(string height, string weight, string medicalHistory)
        {
            if (string.IsNullOrEmpty(height) || string.IsNullOrEmpty(weight) || string.IsNullOrEmpty(medicalHistory))
            {
                return false;
            }

            return true;
        }

        public void Back(object parameter)
        {
            new CrudNurseView(mainStorage).Show();
            this.insertMedicalRecordView.Close();
        }
    }
}
