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
using ZdravoCorp.ViewModel;

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for EquipmentDistributionView.xaml
    /// </summary>
    public partial class EquipmentDistributionView : Window
    {
        public EquipmentDistributionView(MainStorage mainStorage)
        {
            InitializeComponent();
            EquipmentDistributionViewModel eqipmentDistributionViewModel = new EquipmentDistributionViewModel(mainStorage, this);
            eqipmentDistributionViewModel.InitializeComboBox(FromRoomComboBox);
            eqipmentDistributionViewModel.InitializeComboBox(ToRoomComboBox);
            DataContext = eqipmentDistributionViewModel;
        }
    }
}
