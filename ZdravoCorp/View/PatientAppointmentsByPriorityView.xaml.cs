using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for PatientAppointmentsByPriorityView.xaml
    /// </summary>
    public partial class PatientAppointmentsByPriorityView : Window
    {
        public MainStorage MainStorage { get; set; }
        public Patient LoggedPatient { get; set; }

        public PatientAppointmentsView PatientAppointmentsView { get; set; }

        public PatientAppointmentsByPriorityView(MainStorage mainStorage, Patient loggedPatient)
        {
            this.MainStorage = mainStorage;
            this.LoggedPatient = loggedPatient;
            this.PatientAppointmentsView = new PatientAppointmentsView(this.MainStorage, this.LoggedPatient);
            InitializeComponent();
            FillPriorityComboBox();
            FillDoctorsComboBox();

        }

        public void AppointmentByPriority_Click(object sender, RoutedEventArgs e)
        {
            AppointmentsDataGrid.Items.Clear();
            if (!AreDateInputsValid(StartTimeTextBox.Text, EndTimeTextBox.Text, EndDateTextBox.Text))
            {
                return;
            }
            if (!AreItemsSelected())
            {
                MessageBox.Show("Items from both combo boxes must be selected!");
                return;
            }
            DateTime startTime = DateTime.ParseExact(StartTimeTextBox.Text, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(EndTimeTextBox.Text, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(EndDateTextBox.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            Doctor? doctor = FindSelectedDoctor();
            FindAvailableAppointment(doctor!, startTime, endTime, endDate);
        }

        public void FindAvailableAppointment(Doctor doctor, DateTime startTime, DateTime endTime, DateTime endDate)
        {
            DateTime currentDay = DateTime.Today.AddDays(1);
            while (currentDay <= endDate)
            {
                DateTime currentTime = startTime;
                while (startTime <= currentTime && currentTime <= endTime)
                {
                    DateTime appointmentDateTime = currentDay.Date + currentTime.TimeOfDay;
                    DateTime endDateTimeAppointment = appointmentDateTime.AddMinutes(15);
                    if (IsAppointmentAvailable(appointmentDateTime, endDateTimeAppointment, doctor))
                    {
                        CreateNewAppointment(appointmentDateTime, endDateTimeAppointment, doctor);
                        return;
                    }
                    currentTime = currentTime.AddMinutes(1);
                }

                currentDay = currentDay.AddDays(1);
            }
            //IF ARRIVED HERE, THAT MEAN THAT AN AVAILABLE APPOINTMENT WITH INPUT DATA DOES NOT EXIST
            //SO MUST CHECK WHAT IS PRIORITY
            HandleAppointmentNotFound(doctor,startTime, endTime,endDate);

        }
        private void HandleAppointmentNotFound(Doctor doctor, DateTime startTime, DateTime endTime, DateTime endDate)
        {
            string? selectedPriority = PriorityComboBox.SelectedItem.ToString();
            //CHECK PRIORITY
            if (selectedPriority!.Equals("Doctor"))
            {
                FindPriorityDoctorAppointments(doctor, startTime, endTime, endDate);
            }
            else
            {
                FindPriorityAppointmentsInTimeRange(doctor, startTime, endTime, endDate);
            }
        }

        public void FindPriorityDoctorAppointments(Doctor doctor, DateTime startTime, DateTime endTime, DateTime endDate)
        {
            DateTime currentDay = DateTime.Today.AddDays(1);
            DateTime startTimeOfDay = DateTime.Parse("08:00:00");
            DateTime endTimeOfDay = DateTime.Parse("21:00:00");
            while (currentDay <= endDate)
            {
                Appointment? appointment1 = FindAppointmentsInRange(startTimeOfDay, startTime, currentDay, doctor, startTime);
                if (appointment1 != null)
                {
                    PatientAppointmentsView.AddNewAppointment(appointment1, doctor);
                    FillAppointmentsTable(appointment1, doctor);
                    return;
                }

                Appointment? appointment2 = FindAppointmentsInRange(endTime, endTimeOfDay, currentDay, doctor,endTime);
                if (appointment2 != null)
                {
                    PatientAppointmentsView.AddNewAppointment(appointment2, doctor);
                    FillAppointmentsTable(appointment2, doctor);
                    return;
                }
                currentDay = currentDay.AddDays(1);
            }
            //THIRD CASE, WITH PRIORITY NO RESULTS
            FindThreeSimilarAppointmentsOption(doctor, startTime, endTime, endDate);
        }

        public Appointment? FindAppointmentsInRange(DateTime startTime, DateTime endTime, DateTime currentDay, Doctor doctor, DateTime currentTime)
        {
            int minuteStep = GetMinuteStep(endTime, currentTime);
            while (startTime <= currentTime && currentTime <= endTime)
            {
                DateTime appointmentDateTime = currentDay.Date + currentTime.TimeOfDay;
                DateTime endDateTimeAppointment = appointmentDateTime.AddMinutes(15);
                if (IsAppointmentAvailable(appointmentDateTime, endDateTimeAppointment, doctor))
                {
                    string newId = "appointment" + GenerateNewId();
                    return new Appointment(newId, AppointmentType.Appointment, AppointmentStatus.Scheduled, "",
                        new Anamnesis(), new TimeSlot(appointmentDateTime, endDateTimeAppointment));
                }

                currentTime = currentTime.AddMinutes(minuteStep);
            }
            return null;
        }

        public int GetMinuteStep(DateTime startTime, DateTime currentTime)
        {
            if (startTime.Equals(currentTime))
            {
                return -1;
            }
            return 1;
        }

        public void FindPriorityAppointmentsInTimeRange(Doctor doctor, DateTime startTime, DateTime endTime, DateTime endDate)
        {
            DateTime currentDay = DateTime.Today.AddDays(1);
            while (currentDay <= endDate)
            {
                foreach (Doctor storedDoctor in MainStorage.Doctors)
                {
                    if (storedDoctor.Username != doctor.Username) {
                        Appointment? appointment = FindAvailableAppointmentInTimeRange(storedDoctor, startTime, endTime, currentDay);
                        if (appointment != null)
                        {
                            PatientAppointmentsView.AddNewAppointment(appointment, storedDoctor);
                            FillAppointmentsTable(appointment, storedDoctor);
                            return;
                        }
                    }
                }
                currentDay = currentDay.AddDays(1);
            }
            //THIRD CASE, WITH PRIORITY NO RESULTS
            FindThreeSimilarAppointmentsOption(doctor, startTime, endTime, endDate);
        }

        public Appointment? FindAvailableAppointmentInTimeRange(Doctor doctor, DateTime startTime, DateTime endTime, DateTime currentDay)
        {
            DateTime currentTime = startTime;
            while (startTime <= currentTime && currentTime <= endTime)
            {
                DateTime appointmentDateTime = currentDay.Date + currentTime.TimeOfDay;
                DateTime endDateTimeAppointment = appointmentDateTime.AddMinutes(15);
                if (IsAppointmentAvailable(appointmentDateTime, endDateTimeAppointment, doctor))
                {
                    string newId = "appointment" + GenerateNewId();
                    return new Appointment(newId, AppointmentType.Appointment, AppointmentStatus.Scheduled, "",
                        new Anamnesis(), new TimeSlot(appointmentDateTime, endDateTimeAppointment));
                }
                currentTime = currentTime.AddMinutes(1);
            }

            return null;
        }

        public bool IsAppointmentAvailable(DateTime appointmentDateTime, DateTime endDateTimeAppointment, Doctor doctor)
        {
            if (!(this.PatientAppointmentsView.IsDoctorAvailable(appointmentDateTime, endDateTimeAppointment, doctor)))
            {
                return false;
            }
            if (!(this.PatientAppointmentsView.IsPatientAvailable(appointmentDateTime, endDateTimeAppointment)))
            {
                return false;
            }
            return true;
        }


        public void CreateNewAppointment(DateTime appointmentDateTime, DateTime endDateTimeAppointment, Doctor doctor)
        {
            string newId = "appointment" + GenerateNewId();
            Appointment newAppointment = new Appointment(newId, AppointmentType.Appointment, AppointmentStatus.Scheduled, "",
                new Anamnesis(), new TimeSlot(appointmentDateTime, endDateTimeAppointment));
            FillAppointmentsTable(newAppointment, doctor);
            PatientAppointmentsView.AddNewAppointment(newAppointment, doctor);
            if(this.LoggedPatient.Status == Status.Blocked)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Successfully Added New Appointment!");
            }
        }

        public void FindThreeSimilarAppointmentsOption(Doctor doctor, DateTime startTime, DateTime endTime, DateTime endDate)
        {
            List<PatientHistory> patientHistories = new List<PatientHistory>();

            Appointment appointment1 = FindAppointmentAfterEndDate(doctor, startTime, endTime, endDate);
            Console.WriteLine(appointment1.TimeSlot.StartTime);
            Appointment appointment2 = FindAppointmentByAnyDoctorBeforeEndTime(doctor, endDate);
            AppointmentsDataGrid.Items.Add(new PatientHistory(appointment1, doctor));
            AppointmentsDataGrid.Items.Add(new PatientHistory(appointment2, doctor));
            return;

        }

        public Appointment FindAppointmentByAnyDoctorBeforeEndTime(Doctor doctor, DateTime endDate)
        {
            DateTime startTimeOfDay = DateTime.Parse("08:00:00");
            DateTime endTimeOfDay = DateTime.Parse("21:00:00");
            DateTime currentDay = DateTime.Today.AddDays(1);
            while (currentDay <= endDate)
            {
                Appointment appointment = FindAppointmentForDoctor(doctor, currentDay, startTimeOfDay, endTimeOfDay);
                if (appointment != null)
                {
                    return appointment;
                }

                currentDay = currentDay.AddDays(1);
            }

            return null;
        }

        public Appointment FindAppointmentForDoctor(Doctor doctor, DateTime currentDay, DateTime startTimeOfDay, DateTime endTimeOfDay)
        {
            DateTime currentTime = startTimeOfDay;
            while (startTimeOfDay <= currentTime && currentTime <= endTimeOfDay)
            {
                DateTime appointmentDateTime = currentDay.Date + currentTime.TimeOfDay;
                DateTime endDateTimeAppointment = appointmentDateTime.AddMinutes(15);
                if (IsAppointmentAvailable(appointmentDateTime, endDateTimeAppointment, doctor))
                {
                    string newId = "appointment" + GenerateNewId();
                    return new Appointment(newId, AppointmentType.Appointment, AppointmentStatus.Scheduled, "",
                        new Anamnesis(), new TimeSlot(appointmentDateTime, endDateTimeAppointment));
                }
                currentTime = currentTime.AddMinutes(1);
            }

            return null;
        }

        public Appointment FindAppointmentAfterEndDate(Doctor doctor, DateTime startTime, DateTime endTime, DateTime endDate)
        {
            DateTime currentDay = endDate;
            while (currentDay.Year <= DateTime.Now.Year)
            {
                DateTime currentTime = startTime;
                while (startTime <= currentTime && currentTime <= endTime)
                {
                    DateTime appointmentDateTime = currentDay.Date + currentTime.TimeOfDay;
                    DateTime endDateTimeAppointment = appointmentDateTime.AddMinutes(15);
                    if (IsAppointmentAvailable(appointmentDateTime, endDateTimeAppointment, doctor))
                    {
                        string newId = "appointment" + GenerateNewId();
                        return new Appointment(newId, AppointmentType.Appointment, AppointmentStatus.Scheduled, "",
                            new Anamnesis(), new TimeSlot(appointmentDateTime, endDateTimeAppointment));
                    }
                    currentTime = currentTime.AddMinutes(1);
                }

                currentDay = currentDay.AddDays(1);
            }

            return null;
        }

        
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AppointmentsDataGrid.Items.Count == 2)
            {
                var row = sender as DataGridRow;
                PatientHistory? patientHistory = row!.DataContext as PatientHistory;
                PatientAppointmentsView.AddNewAppointment(patientHistory!.Appointment, patientHistory.Doctor);
                AppointmentsDataGrid.Items.Clear();
            }

            MessageBox.Show("Not possible to add that row!");
            
        }

        public Doctor? FindSelectedDoctor()
        {
            string? selectedValue = DoctorsComboBox.SelectedValue.ToString();
            if (!string.IsNullOrEmpty(selectedValue))
            {
                string[] splitValue = selectedValue.Split("-");
                foreach (Doctor doctor in MainStorage.Doctors)
                {
                    if (doctor.Username.Equals(splitValue[0]))
                    {
                        return doctor;
                    }
                }
            }
            return null;
        }

        public bool AreItemsSelected()
        {
            return (DoctorsComboBox.SelectedIndex != -1 && PriorityComboBox.SelectedIndex != -1);
        }

        public bool AreDateInputsValid(string startTimeInput, string endTimeInput, string endDate)
        {
            if (!(PatientAppointmentsView.IsValidTime(startTimeInput, "HH:mm:ss")) || !(PatientAppointmentsView.IsValidTime(endTimeInput, "HH:mm:ss")))
            {
                MessageBox.Show("Invalid start or end time input!");
                return false;
            }
            if (GetTimeDifference(startTimeInput, endTimeInput).Hours < 1)
            {
                MessageBox.Show("End time must be greater for minimum 1 hour!");
                return false;
            }
            if (!(PatientAppointmentsView.IsValidDate(endDate, "yyyy-MM-dd")))
            {
                MessageBox.Show("Invalid end date input!");
                return false;
            }

            if (IsTimeRangeValid(startTimeInput, endTimeInput))
            {
                MessageBox.Show("You can input time between 8:00 and 21:00!");
                return false;
            }

            return true;
        }

        public bool IsTimeRangeValid(string startTimeInput,string endTimeInput)
        {
            if (TimeSpan.TryParseExact(startTimeInput, "HH:mm:ss", null, out TimeSpan startTime)
                && (TimeSpan.TryParseExact(endTimeInput, "HH:mm:ss", null, out TimeSpan endTime)))
            {
                if (startTime >= TimeSpan.FromHours(8) && endTime <= TimeSpan.FromHours(21) && endTime > startTime)
                {
                    return true;
                }
            }
            return false;
        }

        public TimeSpan GetTimeDifference(string startTimeInput, string endTimeInput)
        {
            DateTime startTime = DateTime.ParseExact(startTimeInput, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(endTimeInput, "HH:mm:ss", CultureInfo.InvariantCulture);
            return endTime - startTime;
        }

        public void FillDoctorsComboBox()
        {
            if (MainStorage.Doctors == null)
            {
                MessageBox.Show("Doctors are not available right now!");
                return;
            }
            DoctorsComboBox.Items.Clear();
            foreach (Doctor doctor in MainStorage.Doctors)
            {
                DoctorsComboBox.Items.Add(doctor.Username + "-" + doctor.Specialization);
            }
        }
        public void FillPriorityComboBox()
        {
            PriorityComboBox.Items.Clear();
            PriorityComboBox.Items.Add("Doctor");
            PriorityComboBox.Items.Add("Time interval");
        }
        private void BackMainView_Click(object sender, RoutedEventArgs e)
        {
            MenuPatientView menu = new MenuPatientView(MainStorage, LoggedPatient);
            menu.Show();
            this.Close();
        }

        public int GenerateNewId()
        {
            if (MainStorage.Appointments == null || MainStorage.Appointments.Count == 0)
            {
                return 1;
            }
            List<int> ids = new List<int>();
            foreach (Appointment appointment in MainStorage.Appointments)
            {
                Match match = Regex.Match(appointment.Id, @"\d+$");
                int number = Int32.Parse(match.Value);
                ids.Add(number);
            }

            int newId = ids.Max() + 1;

            return newId;
        }
        public void FillAppointmentsTable(Appointment newAppointment, Doctor doctor)
        {
            PatientHistory patientHistory = new PatientHistory(newAppointment, doctor);
            AppointmentsDataGrid.Items.Add(patientHistory);
        }

    }
}
