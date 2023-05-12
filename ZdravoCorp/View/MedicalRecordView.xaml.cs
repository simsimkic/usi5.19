using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZdravoCorp.Storage;
using ZdravoCorp.ViewModel;
using ZdravoCorp.Model;

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for MedicalRecordView.xaml
    /// </summary>
    public partial class MedicalRecordView : Window
    {
        private MainStorage _mainStorage;
        private string _patientUsername;
        private DoctorAppointmentsView _doctorAppointmentsView;
        private DoctorExaminationView _doctorExaminationView;
        private DoctorPatientListView _doctorPatientListView;
        private CrudNurseView _crudNurseView;
        private Tuple<Appointment, string> _appointmentWithPatientsUsername;

        public MedicalRecordView(MainStorage mainStorage, string patientUsername)
        {
            InitializeComponent();

            _mainStorage = mainStorage;
            _patientUsername = patientUsername;
            DataContext = new MedicalRecordViewModel(mainStorage, patientUsername, this);
        }

        public MedicalRecordView(MainStorage mainStorage, string patientUsername, DoctorAppointmentsView doctorAppointmentsView) : this(mainStorage, patientUsername)
        {
            _doctorAppointmentsView = doctorAppointmentsView;
            DataContext = new MedicalRecordViewModel(mainStorage, patientUsername, this, doctorAppointmentsView);
        }

        public MedicalRecordView(MainStorage mainStorage, string patientUsername, DoctorPatientListView doctorPatientListView) : this(mainStorage, patientUsername)
        {
            _doctorPatientListView = doctorPatientListView;
            DataContext = new MedicalRecordViewModel(mainStorage, patientUsername, this, doctorPatientListView);
        }

        public MedicalRecordView(MainStorage mainStorage, Tuple<Appointment, string> appointmentWithPatientsUsername, DoctorExaminationView doctorExaminationView) : this(mainStorage, appointmentWithPatientsUsername.Item2)
        {
            _doctorExaminationView = doctorExaminationView;
            _appointmentWithPatientsUsername = appointmentWithPatientsUsername;
            DataContext = new MedicalRecordViewModel(mainStorage, appointmentWithPatientsUsername, this, doctorExaminationView);
        }

        public MedicalRecordView(MainStorage mainStorage, string patientUsername, CrudNurseView crudNurseView) : this(mainStorage, patientUsername)
        {
            _crudNurseView = crudNurseView;
            DataContext = new MedicalRecordViewModel(mainStorage, patientUsername, this, crudNurseView);
        }
    }

}
