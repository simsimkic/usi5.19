using System;
using System.Collections.Generic;
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
    public class MedicalRecordViewModel : ViewModelBase
    {
        public MainStorage MainStorage { get; set; }
        public MedicalRecordView MedicalRecordView { get; set; }
        public Patient Patient { get; set; }
        public ICommand BackCommand { get; }

        public DoctorAppointmentsView DoctorAppointmentsView {get; set;}
        public DoctorExaminationView DoctorExaminationView { get; set; }
        public CrudNurseView CrudNurseView { get; set; }

        public Tuple<Appointment, string> AppointmentWithPatientsUsername { get; set; }

        public DoctorPatientListView DoctorPatientListView { get; set; }
        public MedicalRecordViewModel(MainStorage mainStorage, string patientUsername, MedicalRecordView medicalRecordView)
        {
            MainStorage = mainStorage;
            MedicalRecordView = medicalRecordView;
            Patient = getPatientByUsername(patientUsername);
            BackCommand = new RelayCommand(Back);
        }

        public MedicalRecordViewModel(MainStorage mainStorage, string patientUsername, MedicalRecordView medicalRecordView, DoctorAppointmentsView doctorAppointmentsView)
            : this(mainStorage, patientUsername, medicalRecordView)
        {
            DoctorAppointmentsView = doctorAppointmentsView;
        }

        public MedicalRecordViewModel(MainStorage mainStorage, string patientUsername, MedicalRecordView medicalRecordView, CrudNurseView crudNurseView)
            : this(mainStorage, patientUsername, medicalRecordView)
        {
            CrudNurseView = crudNurseView;
        }

        public MedicalRecordViewModel(MainStorage mainStorage, string patientUsername, MedicalRecordView medicalRecordView, DoctorPatientListView doctorPatientListView)
            : this(mainStorage, patientUsername, medicalRecordView)
        {
            DoctorPatientListView = doctorPatientListView;
        }

        public MedicalRecordViewModel(MainStorage mainStorage, Tuple<Appointment, string> appointmentWithPatientsUsername, MedicalRecordView medicalRecordView, DoctorExaminationView doctorExaminationView)
        {
            MainStorage = mainStorage;
            MedicalRecordView = medicalRecordView;
            Patient = getPatientByUsername(appointmentWithPatientsUsername.Item2);
            AppointmentWithPatientsUsername = appointmentWithPatientsUsername;
            DoctorExaminationView = doctorExaminationView;
            BackCommand = new RelayCommand(Back);
        }
        public void Back(object parameter)
        {
            if (this.DoctorAppointmentsView != null)
            {
                new DoctorAppointmentsView(this.MainStorage).Show();
                this.MedicalRecordView.Close();
            }

            if (this.DoctorExaminationView != null)
            {
                new DoctorExaminationView(this.MainStorage, this.AppointmentWithPatientsUsername ).Show();
                this.MedicalRecordView.Close();
            }

            if (this.CrudNurseView != null)
            {
                new CrudNurseView(this.MainStorage).Show();
                this.MedicalRecordView.Close();
            }

            if (this.DoctorPatientListView != null)
            {
                new DoctorPatientListView(this.MainStorage).Show();
                this.MedicalRecordView.Close();
            }

        }


        public Patient getPatientByUsername(string patientUsername)
        {
            return this.MainStorage.Patients.FirstOrDefault(patient => patient.Username == patientUsername);
        }

    }
}
