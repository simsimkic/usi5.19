using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for PatientMedicalRecordView.xaml
    /// </summary>
    public partial class PatientMedicalRecordView : Window
    {
        public MainStorage MainStorage { get; set; }
        public Patient LoggedPatient { get; set; }
        public List<PatientHistory> PatientHistories { get; set; } = new List<PatientHistory>();
        
        public PatientMedicalRecordView(MainStorage mainStorage, Patient loggedPatient)
        {
            InitializeComponent();
            this.MainStorage = mainStorage;
            this.LoggedPatient = loggedPatient;
            List<Appointment>? appointments = FindPatientAppointments();
            if (appointments == null || appointments.Count == 0)
            {
                MessageBox.Show("Logged patient does not have an finished examination!");
                return;
            }
            List<Doctor> doctors = FindDoctorsByAppointmentId(appointments);
            this.PatientHistories = FindPatientHistories(appointments, doctors);
            FillPatientHistoryTable(this.PatientHistories);
            FillSortOption();

        }

        public void FillSortOption()
        {
            SortingComboBox.Items.Add("DEFAULT");
            SortingComboBox.Items.Add("DATE");
            SortingComboBox.Items.Add("DOCTOR");
            SortingComboBox.Items.Add("SPECIALIZATION");
        }

        public List<PatientHistory> FindPatientHistories(List<Appointment> appointments, List<Doctor> doctors)
        {
            List<PatientHistory> patientHistories = new List<PatientHistory>();
            foreach (Appointment appointment in appointments)
            {
                foreach (Doctor doctor in doctors)
                {
                    if (doctor.AppointmentIds.Contains(appointment.Id) && appointment.AppointmentStatus == AppointmentStatus.Finished)
                    {
                        patientHistories.Add(new PatientHistory(appointment, doctor));
                        break;
                    } 
                }

            }
            return patientHistories;

        }
        public List<Doctor> FindDoctorsByAppointmentId(List<Appointment> appointments)
        {
            List<Doctor> doctors = new List<Doctor>();
            foreach (Appointment appointment in appointments)
            {
                Doctor? doctor = FindDoctorByAppointmentId(appointment.Id);
                if (doctor != null)
                {
                    doctors.Add(doctor);
                }
            }

            return doctors;
        }

        public Doctor? FindDoctorByAppointmentId(string id)
        {
            if (MainStorage.Doctors == null)
            {
                return null;
            }

            foreach (Doctor doctor in MainStorage.Doctors)
            {
                if (doctor.AppointmentIds.Contains(id))
                {
                    return doctor;
                }
            }

            return null;
        }

        public List<Appointment>? FindPatientAppointments()
        {
            List<Appointment> appointments = new List<Appointment>();
            foreach (var id in LoggedPatient.MedicalRecord.AppointmentIds)
            {
                Appointment? appointment = FindAppointmentById(id);
                if (appointment == null)
                {
                    return null;
                }

                appointments.Add(appointment);
            }

            return appointments;
        }

        public Appointment? FindAppointmentById(string id)
        {
            if (MainStorage.Appointments == null)
            {
                return null;
            }

            foreach (Appointment appointment in MainStorage.Appointments)
            {
                if (appointment.Id == id)
                {
                    return appointment;
                }
            }

            return null;
        }

        public void FillPatientHistoryTable(List<PatientHistory> patientHistories)
        {
            AppointmentsDataGrid.Items.Clear();
            foreach (PatientHistory patient in patientHistories)
            {
                AppointmentsDataGrid.Items.Add(patient);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchTextBox.Text.ToLower();

            var filteredData = this.PatientHistories.Where(data =>
                (data.Appointment.Anamnesis?.Observations?.ToLower() ?? string.Empty).Contains(keyword) ||
                data.Appointment.TimeSlot.StartTime.ToString("yyyy-MM-dd").Contains(keyword) ||
                data.Doctor.Username.ToLower().Contains(keyword) ||
                data.Doctor.Specialization.ToString().ToLower().Contains(keyword) ||
                (data.Appointment.Anamnesis?.Symptoms != null && data.Appointment.Anamnesis.Symptoms.Any(symptom => symptom.ToLower().Contains(keyword))));

            AppointmentsDataGrid.Items.Clear();
            foreach (var data in filteredData)
            {
                AppointmentsDataGrid.Items.Add(data);
            }

        }

        private void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selectedOption = SortingComboBox.SelectedItem.ToString();

            switch (selectedOption)
            {
                case "DATE":
                    SortByDate();
                    break;

                case "DOCTOR":
                    SortByDoctorUsername();
                    break;

                case "SPECIALIZATION":
                    SortBySpecialization();
                    break;
                case "DEFAULT":
                    SortByAppointmentId();
                    break;
            }
        }
        
        public void SortByAppointmentId()
        {
            this.PatientHistories = this.PatientHistories.OrderBy(data =>
            {
                string id = data.Appointment.Id; 
                string numericPart = id.Substring("appointment".Length); 
                return int.Parse(numericPart);
            }).ToList();
            FillPatientHistoryTable(this.PatientHistories);
        }

        public void SortByDate()
        {
            this.PatientHistories = this.PatientHistories.OrderBy(data => data.Appointment.TimeSlot.StartTime.Date).ToList();
            FillPatientHistoryTable(this.PatientHistories);
        }
        private void SortByDoctorUsername()
        {
            this.PatientHistories = this.PatientHistories.OrderBy(data => data.Doctor.Username).ToList();
            FillPatientHistoryTable(this.PatientHistories);
        }
        private void SortBySpecialization()
        {
            this.PatientHistories = this.PatientHistories.OrderBy(data => data.Doctor.Specialization.ToString()).ToList();
            FillPatientHistoryTable(this.PatientHistories);
        }
        private void BackMainView_Click(object sender, RoutedEventArgs e)
        {
            MenuPatientView menu = new MenuPatientView(MainStorage, LoggedPatient);
            menu.Show();
            this.Close();
        }
    }

}
