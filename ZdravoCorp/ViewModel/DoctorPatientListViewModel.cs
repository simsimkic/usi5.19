using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class DoctorPatientListViewModel : ViewModelBase
    {
        public MainStorage MainStorage { get; set; }
        public DoctorPatientListView DoctorPatientListView { get; set; }

        private ObservableCollection<Patient> _patientsTable;
        public ObservableCollection<Patient> PatientsTable
        {
            get { return _patientsTable; }
            set
            {
                _patientsTable = value;
                OnPropertyChanged(nameof(PatientsTable));
            }
        }
        public Patient SelectedPatient { get; set; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                PerformUpdate(_searchText);
            }
        }
        public ICommand ShowMedicalRecordCommand { get; }
        public ICommand EditMedicalRecordCommand { get; }
        public ICommand BackCommand { get; }

        public DoctorPatientListViewModel(MainStorage mainStorage, DoctorPatientListView doctorPatientListView)
        {
            this.MainStorage = mainStorage;
            this.DoctorPatientListView = doctorPatientListView;
            this.PatientsTable = new ObservableCollection<Patient>(mainStorage.Patients);

            ShowMedicalRecordCommand = new RelayCommand(ShowMedicalRecord);
            EditMedicalRecordCommand = new RelayCommand(EditMedicalRecord);
            BackCommand = new RelayCommand(Back);

        }
        public void Back(object parameter)
        {
            new MenuDoctorView(this.MainStorage).Show();
            this.DoctorPatientListView.Close();
        }
        public void ShowMedicalRecord(object parameter)
        {
            if (this.SelectedPatient != null && IsSelectedPatientBelongsToLoggedDoctor())
            {
                new MedicalRecordView(this.MainStorage, this.SelectedPatient.Username, this.DoctorPatientListView).Show();
                this.DoctorPatientListView.Close();
            }
            else
            {
                MessageBox.Show("Please select a patient assigned to you.");
            }
        }

        public void EditMedicalRecord(object parameter)
        {
            if (SelectedPatient != null && IsSelectedPatientBelongsToLoggedDoctor())
            {
                new UpdateMedicalRecordView(MainStorage, SelectedPatient.Username, DoctorPatientListView).Show();
                DoctorPatientListView.Close();
            }
            else
            {
                MessageBox.Show("Please select a patient assigned to you.");
            }
        }

        private void PerformUpdate(string searchText)
        {
            List<Patient> matchingPatients = MainStorage.Patients.Where(p => p.FirstName.Contains(searchText) || p.LastName.Contains(searchText) || p.Username.Contains(searchText) || p.Status.ToString().Contains(searchText)).ToList();
            PatientsTable = new ObservableCollection<Patient>(matchingPatients);
        }
        private bool IsSelectedPatientBelongsToLoggedDoctor()
        {
            Doctor loggedDoctor = MainStorage.Doctors.FirstOrDefault(doctor => doctor.Username == MainStorage.LoggedPerson.Username);
            if (loggedDoctor != null)
            {
                return loggedDoctor.AppointmentIds.Intersect(SelectedPatient.MedicalRecord.AppointmentIds).Any();
            }

            return false;
        }
    }
}
