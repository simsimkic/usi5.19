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
    public class DoctorExaminationViewModel : ViewModelBase
    {
        public MainStorage MainStorage { get; set; }
        public DoctorExaminationView DoctorExaminationView { get; set; }
        public Tuple<Appointment, string> Appointment { get; set; }

        private string _observation;
        public string Observation
        {
            get { return _observation; }
            set
            {
                if (_observation != value)
                {
                    _observation = value;
                    OnPropertyChanged(nameof(Observation));
                }
            }
        }

        private ObservableCollection<string> _symptoms= new ObservableCollection<string>();
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
        public ICommand BackCommand { get; }
        public ICommand AddSymptomCommand { get; }
        public ICommand DeleteSymptomCommand { get; }
        public ICommand ShowMedicalRecordCommand { get; }
        public ICommand EndExaminationCommand { get; }
        public ICommand EditMedicalRecordCommand { get; }
        public DoctorExaminationViewModel(MainStorage mainStorage, Tuple<Appointment, string> appointment, DoctorExaminationView doctorExaminationView)
        {
            this.MainStorage = mainStorage;
            this.DoctorExaminationView = doctorExaminationView;
            this.Appointment = appointment;

            ShowMedicalRecordCommand = new RelayCommand(ShowMedicalRecord);
            EditMedicalRecordCommand = new RelayCommand(EditMedicalRecord);
            EndExaminationCommand = new RelayCommand(EndExamination);

            BackCommand = new RelayCommand(Back);
            Symptoms = new ObservableCollection<string>(appointment.Item1.Anamnesis.Symptoms);
            AddSymptomCommand = new RelayCommand(AddSymptom);
            DeleteSymptomCommand = new RelayCommand(DeleteSymptom);

            this.Appointment = appointment;
        }

        public void Back(object parameter)
        {
            new DoctorAppointmentsView(this.MainStorage).Show();
            this.DoctorExaminationView.Close();
        }

        public void ShowMedicalRecord(object parameter)
        {
            new MedicalRecordView(this.MainStorage, this.Appointment,DoctorExaminationView ).Show();
            this.DoctorExaminationView.Close();
            
        }

        public void EditMedicalRecord(object parameter)

        {

            new UpdateMedicalRecordView(this.MainStorage, this.Appointment, DoctorExaminationView).Show();
            this.DoctorExaminationView.Close();
               
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
        public void EndExamination(object parameter)
        {
            this.Appointment.Item1.Anamnesis.Observations = this.Observation;
            this.Appointment.Item1.Anamnesis.Symptoms = this.Symptoms.ToList();
            this.Appointment.Item1.AppointmentStatus = AppointmentStatus.Finished;

            this.MainStorage.appointmentStorage.Save(MainStorage.Appointments);

            MessageBox.Show("Examination ended.");

            new DoctorEquipmentUsageView(this.MainStorage, this.Appointment.Item1).Show();
            this.DoctorExaminationView.Close();
        }
    }
}
