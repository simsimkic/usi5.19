using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class CreateAppointmentViewModel : ViewModelBase
    {
        private MainStorage MainStorage { get; set; }
        private CreateAppointmentView CreateAppointmentView{get; set; }

        public List<Patient> PatientsTable { get; set; }

        private string _duration;
        public string Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }


        public IEnumerable<AppointmentType> AppointmentType => Enum.GetValues(typeof(AppointmentType)).Cast<AppointmentType>();
        public AppointmentType SelectedAppointmentType { get; set; }
        public Patient SelectedPatient { get; set; }

        public DateTime SelectedDateTime { get; set; }

        public ICommand BackCommand { get; }
        public ICommand AddCommand { get; }
        public CreateAppointmentViewModel(MainStorage mainStorage, CreateAppointmentView createAppointmentView)
        {
            this.MainStorage = mainStorage;
            this.CreateAppointmentView = createAppointmentView;

            BackCommand = new RelayCommand(Back);
            AddCommand = new RelayCommand(Add);

            this.PatientsTable = mainStorage.Patients;

        }

        public void Add(Object parameter)
        {
            List<string> symptoms = new List<string>();
            Appointment newAppointment = new Appointment();


            if (SelectedPatient == null)
            {
                MessageBox.Show("Please select a patient.");
                return;
            }

            if (!int.TryParse(Duration, out int duration))
            {
                MessageBox.Show("Please enter a valid duration.");
                return;
            }

            if (SelectedAppointmentType == 0 && duration != 15)
            {
                MessageBox.Show("Appointment durations must be 15 minutes.");
                
                return;
            }


            if (SelectedDateTime < DateTime.Now)
            {
                MessageBox.Show("Selected date and time is in the past.");
                return;
            }

            
            if (!isDoctorAvailable(SelectedDateTime, SelectedDateTime.AddMinutes(duration))) 
            {
                MessageBox.Show("You are not available.");
                return;
            }

            if (!isPatientAvailable(SelectedDateTime, SelectedDateTime.AddMinutes(duration), SelectedPatient))
            {
                MessageBox.Show("Patient is not available.");
                return;
            }

            newAppointment.Id = NextId();
            newAppointment.AppointmentType = SelectedAppointmentType;
            newAppointment.AppointmentStatus = AppointmentStatus.Scheduled;

           
            //DateTime dateTime = DateTime.ParseExact(date, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

            newAppointment.Anamnesis = new Anamnesis("", symptoms);

           

            newAppointment.TimeSlot = new TimeSlot(SelectedDateTime, SelectedDateTime.AddMinutes(duration));
            MainStorage.Appointments.Add(newAppointment);

            addAppointmentToDoctor(newAppointment.Id);
            addAppointmentToPatient(newAppointment.Id, SelectedPatient.Username);

            MainStorage.appointmentStorage.Save(MainStorage.Appointments);
            MessageBox.Show($"Appointment with ID {newAppointment.Id} has been successfully added");
        }



        public void addAppointmentToDoctor(String appointmentId)
        {
            Doctor loggedDoctor = new Doctor();
            foreach (Doctor doctor in MainStorage.Doctors)
            {
                if (doctor.Username == this.MainStorage.LoggedPerson.Username)
                {
                    loggedDoctor = doctor;
                }
            }

            loggedDoctor.AppointmentIds.Add(appointmentId);
            MainStorage.doctorStorage.Save(MainStorage.Doctors);
        }

        public void addAppointmentToPatient(String appointmentId, string patientUsername)
        {
            
            foreach (Patient patient in MainStorage.Patients)
            {
                if (patient.Username == patientUsername)
                {
                    patient.MedicalRecord.AppointmentIds.Add(appointmentId);
                    //patient.AppointmentIds.Add(appointmentId);
                    MainStorage.patientStorage.Save(MainStorage.Patients);
                }
            }

            
        }

        public string NextId()
        {
            string lastId = MainStorage.Appointments.LastOrDefault()?.Id;
            if (lastId == null)
            {
                return "appointment1";
            }
            else
            {
                int lastIdNumber = int.Parse(lastId.Replace("appointment", ""));
                return $"appointment{lastIdNumber + 1}";
            }
        }
        
        public bool isDoctorAvailable(DateTime startDateTime, DateTime endDateTime)
        {
            Doctor loggedDoctor = new Doctor();
            foreach (Doctor doctor in MainStorage.Doctors)
            {
                if (doctor.Username == this.MainStorage.LoggedPerson.Username)
                {
                    loggedDoctor = doctor;
                }
            }

            foreach (string appointmentIds in loggedDoctor.AppointmentIds)
            {
                Appointment app = findAppointmentById(appointmentIds);
                if (app.AppointmentStatus == AppointmentStatus.Scheduled)
                {
                    if (startDateTime >= app.TimeSlot.StartTime && startDateTime <= app.TimeSlot.EndTime)
                    {
                        return false;
                    }

                    if (endDateTime >= app.TimeSlot.StartTime && endDateTime <= app.TimeSlot.EndTime)
                    {
                        return false;
                    }
                }
                
            }

            foreach (string free in loggedDoctor.FreeDaysIds)
            {
                FreeDays doctorFree = findDoctorFreeDays(free);
                if (doctorFree != null)
                {
                    if (startDateTime >= doctorFree.TimeSlot.StartTime && startDateTime <= doctorFree.TimeSlot.EndTime)
                    {
                        return false;
                    }
                    if (endDateTime >= doctorFree.TimeSlot.StartTime && endDateTime <= doctorFree.TimeSlot.EndTime)
                    {
                        return false;
                    }

                }
            }

            return true;
        }

        public bool isPatientAvailable(DateTime startDateTime, DateTime endDateTime, Patient selectedPatient)
        {
            //Patient spatient = new Patient();

            foreach (Patient patient in MainStorage.Patients)
            {
                if (selectedPatient.Username == patient.Username)
                {
                    selectedPatient = patient;
                }
            }

            
            foreach (string appointmentIds in selectedPatient.MedicalRecord.AppointmentIds)
            {
                Appointment app = findAppointmentById(appointmentIds);
                if (app.AppointmentStatus == AppointmentStatus.Scheduled)
                {
                    if (startDateTime >= app.TimeSlot.StartTime && startDateTime <= app.TimeSlot.EndTime)
                    {
                        return false;
                    }

                    if (endDateTime >= app.TimeSlot.StartTime && endDateTime <= app.TimeSlot.EndTime)
                    {
                        return false;
                    }
                }

            }

            return true;

        }

        public FreeDays findDoctorFreeDays(string id)
        {
            FreeDays free = new FreeDays();
            foreach (FreeDays fr in MainStorage.FreeDays)
            {
                if (id == fr.Id)
                {
                    free = fr;
                    break;

                }
            }

            return free;
        }
        public Appointment findAppointmentById(string id)
        {
            Appointment app = new Appointment();
            foreach (Appointment appss in MainStorage.Appointments)
            {
                if (appss.Id == id)
                {
                    app = appss;
                    break;
                }
            }

            return app;
        }
        public void Back(object parameter)
        {
            new DoctorAppointmentsView(this.MainStorage).Show();
            this.CreateAppointmentView.Close();
        }
    }
}
