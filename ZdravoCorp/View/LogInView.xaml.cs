using System;
using System.Windows;
using ZdravoCorp.Storage;
using ZdravoCorp.ViewModel;

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for LogInView.xaml
    /// </summary>
    public partial class LogInView : Window
    {   
       
        public LogInView()
        {
            InitializeComponent();

            MainStorage mainStorage = new MainStorage();
            mainStorage.loadAllData();
            DataContext = new LoginViewModel(mainStorage, this);


        }
    }
}
