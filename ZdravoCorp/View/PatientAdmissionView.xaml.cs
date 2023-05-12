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
    /// Interaction logic for PatientAdmissionView.xaml
    /// </summary>
    public partial class PatientAdmissionView : Window
    {
        private MainStorage mainStorage;
        private Patient selectedPatient;
        private Appointment patientsAppointment;

        public PatientAdmissionView(MainStorage mainStorage, Patient selectedPatient, Appointment patientsAppointment)
        {
            InitializeComponent();

            this.mainStorage = mainStorage;
            this.selectedPatient = selectedPatient;
            this.patientsAppointment = patientsAppointment;
            DataContext = new PatientAdmissionViewModel(mainStorage, selectedPatient, patientsAppointment, this);
        }

    
    }
}
