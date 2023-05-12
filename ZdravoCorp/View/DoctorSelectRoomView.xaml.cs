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
    /// Interaction logic for DoctorSelectRoomView.xaml
    /// </summary>
    public partial class DoctorSelectRoomView : Window
    {
        private MainStorage mainStorage;
        private Tuple<Appointment, string> appointment;
        public DoctorSelectRoomView(MainStorage mainStorage, Tuple<Appointment, string> appointment)
        {
            InitializeComponent();
            this.mainStorage = mainStorage;
            this.appointment = appointment;

            DataContext = new DoctorSelectRoomViewModel(mainStorage, appointment, this);

        }
    }
}
