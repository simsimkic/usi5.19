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
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.ViewModel;

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for UpdateMedicalRecordView.xaml
    /// </summary>
    public partial class UpdateMedicalRecordView : Window
    {
        private MainStorage mainStorage;
        private string patientUsername;
        private DoctorPatientListView doctorPatientListView;
        private DoctorExaminationView doctorExaminationView;
        private Tuple<Appointment, string> appointmentWithPatientsUsername;

        public UpdateMedicalRecordView(MainStorage mainStorage, string patientUsername)
        {
            InitializeComponent();
            this.mainStorage = mainStorage;
            this.patientUsername = patientUsername;
            DataContext = new UpdateMedicalRecordViewModel(mainStorage, patientUsername, this);
        }

        public UpdateMedicalRecordView(MainStorage mainStorage, string patientUsername, DoctorPatientListView doctorPatientListView)
            : this(mainStorage, patientUsername)
        {
            InitializeComponent();
            this.doctorPatientListView = doctorPatientListView;
            DataContext = new UpdateMedicalRecordViewModel(mainStorage, patientUsername, this, doctorPatientListView);
        }

        public UpdateMedicalRecordView(MainStorage mainStorage, Tuple<Appointment, string> appointmentWithPatientsUsername, DoctorExaminationView doctorExaminationView)
        {
            InitializeComponent();
            this.mainStorage = mainStorage;
            this.appointmentWithPatientsUsername = appointmentWithPatientsUsername;
            this.doctorExaminationView = doctorExaminationView;
            DataContext = new UpdateMedicalRecordViewModel(mainStorage, appointmentWithPatientsUsername, this, doctorExaminationView);
        }
    }

}
