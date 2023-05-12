using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using ZdravoCorp.Model;
using ZdravoCorp.View;
using ZdravoCorp.Commands;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using ZdravoCorp.Serializer;
using ZdravoCorp.Storage;

namespace ZdravoCorp.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public MainStorage MainStorage { get; set; }
        public LogInView LogInView { get; set; }

        public ICommand LoginCommand { get; }
        public LoginViewModel(MainStorage mainStorage, LogInView logInView)
        {
            this.MainStorage = mainStorage;
            this.LogInView = logInView;
            LoginCommand = new RelayCommand((param) => LoggedIn(param));
        }

        public void showNotifications(string usernameInput)
        {
            foreach (Notification notification in this.MainStorage.Notifications)
            {
                if (notification.PersonUsername == usernameInput && notification.NotificationStatus == NotificationStatus.NotShowed)
                {
                    notification.NotificationStatus = NotificationStatus.Showed;

                    MessageBox.Show($"{notification.Title} \n {notification.Description}");

                    this.MainStorage.notificationStorage.Save(this.MainStorage.Notifications);
                }
            }
        }

        private void LoggedIn(object parameters)
        {
            var tuple = (Tuple<string, string>)parameters;
            string usernameInput = (string)tuple.Item1;
            string passwordInput = (string)tuple.Item2;

            new Order().ChangeFileAfter24Hours(this.MainStorage);
            new Transfer().ChangeFileDeferred(this.MainStorage);

            for (int i = 0; i < MainStorage.Doctors.Count; i++)
            {
                if (MainStorage.Doctors[i].Username == usernameInput && MainStorage.Doctors[i].Password == passwordInput)
                {
                    MainStorage.LoggedPerson = (Doctor)MainStorage.Doctors[i];
                    MenuDoctorView menuDoctorView = new MenuDoctorView(MainStorage);
                    
                    this.showNotifications(usernameInput);
                    LogInView.Close();
                        
                    menuDoctorView.Show();


                    return;
                }

            }

            

            for (int i = 0; i < MainStorage.Nurses.Count; i++)
            {
                if (MainStorage.Nurses[i].Username == usernameInput && MainStorage.Nurses[i].Password == passwordInput)
                {
                    MainStorage.LoggedPerson = MainStorage.Nurses[i];
                    MenuNurseView menuNurseView = new MenuNurseView(MainStorage);
                    LogInView.Close();
                    menuNurseView.Show();
                    return;
                }

            }


            for (int i = 0; i < MainStorage.Administrators.Count; i++)
            {
                if (MainStorage.Administrators[i].Username == usernameInput && MainStorage.Administrators[i].Password == passwordInput)
                {
                    MainStorage.LoggedPerson = MainStorage.Administrators[i];
                    MenuAdministratorView menuAdministratorView = new MenuAdministratorView(MainStorage);
                    LogInView.Close();
                    menuAdministratorView.Show();
                    return;
                }

            }


            for (int i = 0; i < MainStorage.Patients.Count; i++)
            {
                if (MainStorage.Patients[i].Username == usernameInput && MainStorage.Patients[i].Password == passwordInput)
                {
                    if (MainStorage.Patients[i].Status == Status.Blocked)
                    {
                        MessageBox.Show("Your account is blocked!");
                        return;
                    }
                    MainStorage.LoggedPerson = MainStorage.Patients[i];
                    MenuPatientView menuPatientView = new MenuPatientView(MainStorage, MainStorage.Patients[i]);

                    this.showNotifications(usernameInput);
                    LogInView.Close();
                    menuPatientView.Show();


                    return;
                }

            }

            MessageBox.Show($"Check again! Your username or password is WRONG!");

        }
        
    }
}
