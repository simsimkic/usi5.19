using System;
using System.Collections.Generic;
using System.Data;
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

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for EquipmentDisplayView.xaml
    /// </summary>
    public partial class EquipmentDisplayView : Window
    {
        public MainStorage MainStorage { get; set; }
        public EquipmentDisplayView(MainStorage mainStorage)
        {
            InitializeComponent();
            this.MainStorage = mainStorage;
            DataContext = new EquipmentDisplayViewModel(mainStorage, this);
        }

    }
}
