using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for InsertPatientView.xaml
    /// </summary>
    public partial class InsertPatientView : Window
    {
        private MainStorage mainStorage;
        private CrudNurseView crudNurseView;

        public InsertPatientView(MainStorage mainStorage, CrudNurseView crudNurseView)
        {
            InitializeComponent();
            this.mainStorage = mainStorage;
            this.crudNurseView = crudNurseView;
            DataContext = new InsertPatientViewModel(mainStorage, crudNurseView, this);
        }

    }
}
