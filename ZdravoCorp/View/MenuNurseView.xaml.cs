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
    /// Interaction logic for MenuNurseView.xaml
    /// </summary>
    public partial class MenuNurseView : Window
    {
        public MainStorage MainStorage { get; set; }
        public MenuNurseView(MainStorage mainStorage)
        {
            InitializeComponent();
            this.MainStorage = mainStorage;

            DataContext = new MenuNurseViewModel(mainStorage, this);
        }

        //private void BtnPatients(object sender, RoutedEventArgs e)
        //{
        //    new CrudNurseView(this.MainStorage).Show();
        //    this.Close();
        //    //Poziva se View koji se otvara klikom na izabrano dugme,
        //    //Parametri koji se prosledjuju u konstruktor su mainStorage (ucitani podaci),
        //    //i this (da bismo omogucili back dugme na novom View)
        //}

        //private void BtnBack(object sender, RoutedEventArgs e)
        //{
        //    new LogInView().Show();
        //    this.Close();
        //}
    }
}
