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
using ZdravoCorp.Model;
using ZdravoCorp.ViewModel;

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for DistributionOrderView.xaml
    /// </summary>
    public partial class DistributionOrderView : Window
    {
        public DistributionOrderView(MainStorage mainStorage, Transfer transfer)
        {
            InitializeComponent();
            DataContext = new DistributionOrderViewModel(mainStorage, this, transfer);
        }
    }
}
