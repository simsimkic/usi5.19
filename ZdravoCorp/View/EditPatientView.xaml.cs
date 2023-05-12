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
    /// Interaction logic for EditPatientView.xaml
    /// </summary>
    public partial class EditPatientView : Window
    {
        private MainStorage mainStorage { get; set; }
        private CrudNurseView crudNurseView { get; set; }
        private Patient patient { get; set; }

        public EditPatientView(MainStorage mainStorage, Patient patient, CrudNurseView crudNurseView)
        {
            InitializeComponent();
            this.mainStorage = mainStorage;
            this.patient = patient;
            this.crudNurseView = crudNurseView;
            DataContext = new EditPatientViewModel(mainStorage, patient, crudNurseView, this);

        }
    }
}
