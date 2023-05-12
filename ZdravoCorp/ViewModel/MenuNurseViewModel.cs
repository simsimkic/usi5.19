using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZdravoCorp.Commands;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class MenuNurseViewModel :ViewModelBase
    {
        private MainStorage mainStorage { get; set; }
        private MenuNurseView menuNurseView { get; set; }
        public ICommand PatientActionsCommand { get; }
        public ICommand CreateEmergencyAppotinmentCommand { get; }
        public ICommand LogOutCommand { get; }

        public MenuNurseViewModel(MainStorage mainStorage, MenuNurseView menuNurseView)
        {
            this.mainStorage = mainStorage;
            PatientActionsCommand = new RelayCommand(StartPatientActions);
            CreateEmergencyAppotinmentCommand = new RelayCommand(CreateEmergencyAppointment);
            LogOutCommand = new RelayCommand(LogOut);
            this.menuNurseView = menuNurseView;
        }

        public void StartPatientActions(object parameter)
        {
            new CrudNurseView(mainStorage).Show();
            this.menuNurseView.Close();
        }

        public void CreateEmergencyAppointment(object paramete)
        {
            new CreateEmergencyAppointmentView(mainStorage).Show();
            this.menuNurseView.Close();
        }

        public void LogOut(object parameter)
        {
            new LogInView().Show();
            this.menuNurseView.Close();
        }
    }
}
