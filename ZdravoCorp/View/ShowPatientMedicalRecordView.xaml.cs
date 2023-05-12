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

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for ShowPatientMedicalRecordView.xaml
    /// </summary>
    public partial class ShowPatientMedicalRecordView : Window
    {
        private MainStorage mainStorage { get; set; }
        private Patient patient;

        public ShowPatientMedicalRecordView(MainStorage mainStorage, Patient patient)
        {
            InitializeComponent();
            this.mainStorage = mainStorage;
            this.patient = patient;


            medicalHistoryDataGrid.Items.Clear();
            medicalHistoryDataGrid.ItemsSource = patient.MedicalRecord.MedicalHistory;

            height_txt.Text = patient.MedicalRecord.Height.ToString();
            weight_txt.Text = patient.MedicalRecord.Weight.ToString();
            username_txt.Text = patient.Username;
        }

        private void BtnBack(object sender, RoutedEventArgs e)
        {
            new CrudNurseView(this.mainStorage).Show();
            this.Close();
        }
    }
}
