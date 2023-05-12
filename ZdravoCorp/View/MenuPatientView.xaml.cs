using System.Windows;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;


namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for MenuDoctorView.xaml
    /// </summary>
    public partial class MenuPatientView : Window
    {
        public MainStorage MainStorage {  get; set; }
        public Patient LoggedPatient { get; set; }
        public MenuPatientView(MainStorage mainStorage, Patient loggedPatient)
        {
            InitializeComponent();
            this.MainStorage = mainStorage;
            this.LoggedPatient = loggedPatient;
            //this.Show();
        }

        private void BtnAppointmentWithPriority(object sender, RoutedEventArgs e)
        {
            PatientAppointmentsByPriorityView patientAppointmentsByPriorityView =
                new PatientAppointmentsByPriorityView(this.MainStorage, this.LoggedPatient);
            patientAppointmentsByPriorityView.Show();
            this.Close();
        }


        private void BtnPatientMedicalHistory(object sender, RoutedEventArgs e)
        {
            PatientMedicalRecordView patientMedicalRecordView = new PatientMedicalRecordView(this.MainStorage, this.LoggedPatient);
            patientMedicalRecordView.Show();
            this.Close();
        }
        private void BtnAppointments(object sender, RoutedEventArgs e)
        {
            PatientAppointmentsView appointmentView = new PatientAppointmentsView(this.MainStorage, this.LoggedPatient);
            appointmentView.Show();
            this.Close();
        }

        private void BtnBack(object sender, RoutedEventArgs e)
        {
            new LogInView().Show();
            this.Close();
        }
    }
}
