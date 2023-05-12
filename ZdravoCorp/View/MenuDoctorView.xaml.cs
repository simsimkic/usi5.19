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

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for MenuDoctorView.xaml
    /// </summary>
    public partial class MenuDoctorView : Window
    {
        public MainStorage MainStorage {  get; set; }
        public MenuDoctorView(MainStorage mainStorage)
        {
            InitializeComponent();
            this.MainStorage = mainStorage;
        }

        private void BtnAppointments(object sender, RoutedEventArgs e)
        {
            new DoctorAppointmentsView(this.MainStorage).Show();
            this.Close();
        }

        private void BtnBack(object sender, RoutedEventArgs e)
        {
            
            new LogInView().Show();
            this.Close();
        }

        private void BtnPatients(object sender, RoutedEventArgs e)
        {
            new DoctorPatientListView(this.MainStorage).Show();
            this.Close();
        }
    }
}
