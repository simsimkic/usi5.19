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
    /// Interaction logic for UpdateAppointmentView.xaml
    /// </summary>
    public partial class UpdateAppointmentView : Window
    {
        private MainStorage mainStorage;
        private Tuple<Appointment, string> selectedAppointment;

        public UpdateAppointmentView(MainStorage mainStorage, Tuple<Appointment, string> selectedAppointment)
        {
            InitializeComponent();

            this.mainStorage = mainStorage;
            this.selectedAppointment = selectedAppointment;

            DataContext = new UpdateAppointmentViewModel(mainStorage, selectedAppointment, this);
        }

        
    }
}
