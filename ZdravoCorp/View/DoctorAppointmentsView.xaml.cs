﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
    /// Interaction logic for DoctorAppointmentsView.xaml
    /// </summary>
    public partial class DoctorAppointmentsView : Window
    {
        private MainStorage mainStorage;
  

        public DoctorAppointmentsView(MainStorage mainStorage)
        {
            InitializeComponent();
            this.mainStorage = mainStorage;

            
            DataContext = new DoctorAppointmentsViewModel(mainStorage, this);
          

        }

    }
}

