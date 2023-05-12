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
    /// Interaction logic for InsertMedicalRecordView.xaml
    /// </summary>
    public partial class InsertMedicalRecordView : Window
    {
        private MainStorage mainStorage { get; set; }
        private CrudNurseView crudNurseView { get; set; }
        private Patient newPatient;

        public InsertMedicalRecordView(MainStorage mainStorage, Patient newPatient, CrudNurseView crudNurseView)
        {
            InitializeComponent();
            this.mainStorage = mainStorage;
            this.crudNurseView = crudNurseView;
            this.newPatient = newPatient;
            DataContext = new InsertMedicalRecordViewModel(mainStorage, crudNurseView, newPatient, this);
        }
 
    }
}
