using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;

namespace ZdravoCorp.View
{
    /// <summary>
    /// Interaction logic for PatientAppointmentsView.xaml
    /// </summary>
    public partial class PatientAppointmentsView : Window
    {
        public MainStorage MainStorage { get; set; }
        public Patient LoggedPatient { get; set; }

        public PatientAppointmentsView(MainStorage mainStorage, Patient loggedPatient)
        {
            InitializeComponent();
            this.MainStorage = mainStorage;
            this.LoggedPatient = loggedPatient;
            List<string> appointmentsIds = LoggedPatient.MedicalRecord.AppointmentIds;
            FillPatientAppointmentTable(appointmentsIds);
            FillComboBox();
            
        }

        public void FillComboBox()
        {
            if (MainStorage.Doctors is null)
                return;

            foreach (var doctor in MainStorage.Doctors)
            {
                string username = doctor.Username;
                string specialization = doctor.Specialization.ToString();
                string item = $"{char.ToUpper(username[0])}{username.Substring(1)}-{specialization}";
                DoctorCombOBox.Items.Add(item);
            }
        }

        public void FillPatientAppointmentTable(List<string> patientAppointments)
        {
            appointmentsDataGrid.Items.Clear();

            if (MainStorage.Appointments is null)
                return;

            foreach (Appointment appointment in MainStorage.Appointments)
            {
                if (patientAppointments.Contains(appointment.Id) &&
                    appointment.AppointmentStatus != AppointmentStatus.Canceled)
                {
                    appointmentsDataGrid.Items.Add(appointment);
                }
            }
        }

        public static bool IsValidDate(string date, string dateFormat)
        {
            DateTime result;
            return DateTime.TryParseExact(date, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out result) && result >= DateTime.Now;
        }

        public static bool IsValidTime(string time, string timeFormat)
        {
            DateTime result;
            return DateTime.TryParseExact(time, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out result);
        }

        private void CreateAppointment(string date, string time, Doctor doctor, string dateFormat, string timeFormat)
        {

            DateTime appointmentDateTime = GetAppointmentDateTime(date, time, dateFormat, timeFormat);
            DateTime endDateTimeAppointment = appointmentDateTime.AddMinutes(15);

            if (!IsDoctorAvailable(appointmentDateTime, endDateTimeAppointment, doctor))
            {
                MessageBox.Show("Selected doctor is not available for selected date and time!");
                return;
            }

            if (!IsPatientAvailable(appointmentDateTime, endDateTimeAppointment))
            {
                MessageBox.Show("This patient have appointment on this day in selected time!");
                return;
            }

            string newId = "appointment" + GenerateNewId();
            
            Appointment newAppointment = new Appointment(newId, AppointmentType.Appointment, AppointmentStatus.Scheduled,"",
                new Anamnesis(), new TimeSlot(appointmentDateTime, endDateTimeAppointment));
            AddNewAppointment(newAppointment, doctor);

        }

        public static DateTime GetAppointmentDateTime(string date, string time, string dateFormat, string timeFormat)
        {
            DateTime dateAppointment = DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture);
            DateTime timeAppointment = DateTime.ParseExact(time, timeFormat, CultureInfo.InvariantCulture);
            return dateAppointment.Date + timeAppointment.TimeOfDay;
        }

        public bool IsPatientAvailable(DateTime startTime, DateTime endTime)
        {
            if (LoggedPatient == null) { return false; }
            foreach (string id in LoggedPatient.MedicalRecord.AppointmentIds)
            {
                Appointment? appointment = FindAppointmentById(id);
                if (startTime >= appointment.TimeSlot.StartTime && startTime <= appointment.TimeSlot.EndTime)
                {
                    return false;
                }

                if (endTime >= appointment.TimeSlot.StartTime && endTime <= appointment.TimeSlot.EndTime)
                {
                    return false;
                }

            }
            return true;
        }
        public void AddNewAppointment(Appointment appointment, Doctor doctor)
        {
            if (MainStorage.Appointments is null || MainStorage.Doctors is null || MainStorage.Patients is null)
                return;

            AddAppointment(appointment, doctor);
            CreatePatientAppointmentAction();
            SaveAppointmentData();
            RefreshAppointmentsTable();
            CheckPatientActions();
        }

        private void AddAppointment(Appointment app, Doctor doctor)
        {
            MainStorage.Appointments.Add(app);
            LoggedPatient.MedicalRecord.AppointmentIds.Add(app.Id);
            doctor.AppointmentIds.Add(app.Id);
        }

        private void SaveAppointmentData()
        {

            MainStorage.appointmentStorage.Save(MainStorage.Appointments);
            MainStorage.patientStorage.Save(MainStorage.Patients);
            MainStorage.doctorStorage.Save(MainStorage.Doctors);
            MainStorage.patientAppointmentActionsStorage.Save(MainStorage.PatientAppointmentActions);
        }

        private void RefreshAppointmentsTable()
        {
            appointmentsDataGrid.Items.Clear();
            FillPatientAppointmentTable(LoggedPatient.MedicalRecord.AppointmentIds);
        }

        private void CreatePatientAppointmentAction()
        {
            PatientAppointmentActions patientAppointmentActions = new PatientAppointmentActions(
                LoggedPatient.Username, PatientAction.Create, DateTime.Now);
            MainStorage.PatientAppointmentActions.Add(patientAppointmentActions);
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

        public Appointment? FindAppointmentById(string id)
        {
            if (MainStorage.Appointments == null)
            {
                return null;
            }
            foreach (Appointment appointment in MainStorage.Appointments)
            {
                if (appointment.Id == id)
                {
                    return appointment;
                }
            }

            return null;
        }
        public bool IsDoctorAvailable(DateTime startDateTime, DateTime endDateTime, Doctor doctor)
        {
            if (IsDoctorOccupiedDuringAppointments(startDateTime, endDateTime, doctor))
            {
                return false;
            }

            if (IsDoctorHasFreeDays(startDateTime, endDateTime, doctor))
            {
                return false;
            }

            return true;
        }

        private bool IsDoctorOccupiedDuringAppointments(DateTime startDateTime, DateTime endDateTime, Doctor doctor)
        {
            foreach (string appointmentId in doctor.AppointmentIds)
            {
                Appointment? appointment = FindAppointmentById(appointmentId);
                if (IsTimeSlotOverlapping(startDateTime, endDateTime, appointment!.TimeSlot))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsDoctorHasFreeDays(DateTime startDateTime, DateTime endDateTime, Doctor doctor)
        {
            foreach (string freeDayId in doctor.FreeDaysIds)
            {
                FreeDays freeDay = FindDoctorFreeDays(freeDayId);
                if (IsTimeSlotOverlapping(startDateTime, endDateTime, freeDay.TimeSlot))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsTimeSlotOverlapping(DateTime startDateTime, DateTime endDateTime, TimeSlot timeSlot)
        {
            return startDateTime >= timeSlot.StartTime && startDateTime <= timeSlot.EndTime ||
                   endDateTime >= timeSlot.StartTime && endDateTime <= timeSlot.EndTime;
        }


        public FreeDays FindDoctorFreeDays(string id)
        {
            foreach (FreeDays freeDay in MainStorage.FreeDays)
            {
                if (id == freeDay.Id)
                {
                    return freeDay;
                }
            }

            return null;
        }

        private void CreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            string dateFormat = "yyyy-MM-dd";
            string timeFormat = "HH:mm:ss";
            string date = dateInput_TextBox.Text.Trim();
            string time = timeInput_TextBox.Text.Trim();

            if (!IsValidDate(date, dateFormat) || !IsValidTime(time, timeFormat))
            {
                MessageBox.Show("Invalid date or time input!");
                return;
            }

            string? doctorSelection = DoctorCombOBox.SelectionBoxItem as string;
            if (string.IsNullOrEmpty(doctorSelection))
            {
                MessageBox.Show("Doctor not selected!");
                return;
            }

            Doctor doctor = FindSelectedDoctor(doctorSelection);

            CreateAppointment(date, time, doctor, dateFormat, timeFormat);
        }

        public Doctor FindSelectedDoctor(String doctorSelection)
        {
            string[] splitSelection = doctorSelection.Split('-');
            string doctorUsername = splitSelection[0];
            Doctor doctor = FindDoctorByUsername(doctorUsername);
            return doctor;
        }

        public Doctor FindDoctorByUsername(string doctorUsername)
        {

            doctorUsername = doctorUsername.ToLower();
            foreach (Doctor doctor in MainStorage.Doctors)
            {

                if (doctor.Username == doctorUsername)
                {
                    return doctor;
                }
            }

            return null;
        }

        private void DoctorCombOBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // morala bi biti globalna neka promenljiva ovde pa da se na svaku
            //promenu menja vrednost, razmisliti o tome kasnije
            //string selectedItem = DoctorCombOBox.SelectedItem as string;
        }
        public bool CheckIsDate24HDiff(Appointment appointment)
        {
            DateTime now = DateTime.Now;

            // calculate the end of the 24 hour period
            DateTime endOf24Hrs = now.AddDays(1);

            // check if the appointment starts within the next 24 hours
            if (appointment.TimeSlot.StartTime >= now && appointment.TimeSlot.StartTime < endOf24Hrs)
            {
                return false;
            }
            return true;
        }

        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (appointmentsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("You must select a row first!");
                return;
            }

            Appointment? appointment = appointmentsDataGrid.SelectedItem as Appointment;

            if (!CheckIsDate24HDiff(appointment))
            {
                MessageBox.Show("It's impossible to delete the appointment because it's only one day before starting!");
                return;
            }

            CancelAppointment(appointment);
            UpdateAppointmentTable();
            PerformPatientAppointmentActions();
        }

        private void CancelAppointment(Appointment appointment)
        {
            appointment.AppointmentStatus = AppointmentStatus.Canceled;
            MainStorage.appointmentStorage.Save(MainStorage.Appointments);
        }

        private void UpdateAppointmentTable()
        {
            appointmentsDataGrid.Items.Clear();
            FillPatientAppointmentTable(LoggedPatient.MedicalRecord.AppointmentIds);
        }

        private void PerformPatientAppointmentActions()
        {
            PatientAppointmentActions? patientAppointmentActions =
                new PatientAppointmentActions(LoggedPatient.Username, PatientAction.Delete, DateTime.Now);
            MainStorage.PatientAppointmentActions.Add(patientAppointmentActions);
            MainStorage.patientAppointmentActionsStorage.Save(MainStorage.PatientAppointmentActions);
        }



        private void BackMainView_Click(object sender, RoutedEventArgs e)
        {
            MenuPatientView menu = new MenuPatientView(MainStorage, LoggedPatient);
            menu.Show();
            this.Close();
        }
        private void UpdateAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (appointmentsDataGrid.SelectedItem != null)
            {
                Appointment? appointment = appointmentsDataGrid.SelectedItem as Appointment;
                string? selectedDoctor = DoctorCombOBox.SelectedItem as string;
                string date = dateInput_TextBox.Text;
                string time = timeInput_TextBox.Text;
                if (!CheckIsDate24HDiff(appointment))
                {
                    MessageBox.Show("It's impossible to update appointment because it's only one day before starting!");
                    return;
                }
                if (!IsValidDate(date, "yyyy-MM-dd") || !IsValidTime(time, "HH:mm:ss"))
                {
                    MessageBox.Show("Invalid date or time input!");
                    return;
                }
                if (string.IsNullOrEmpty(selectedDoctor))
                {
                    MessageBox.Show("You must select doctor!");
                    return;
                }
                Doctor? newDoctor = FindSelectedDoctor(selectedDoctor);

                string idAppointment = appointment.Id;
                DateTime startTimeAppointment = GetAppointmentDateTime(date, time, "yyyy-MM-dd", "HH:mm:ss");
                DateTime endTimeAppointment = startTimeAppointment.AddMinutes(15);
                Doctor doctor = FindDoctorByAppointmentId(idAppointment);

                if (!IsDoctorUpdatedOrAvailable(doctor, newDoctor, appointment, startTimeAppointment, endTimeAppointment))
                {
                    MessageBox.Show("Selected doctor is not available for selected date and time!");
                    return;
                }
                UpdateAppointment(appointment, startTimeAppointment, endTimeAppointment);
                UpdatePatientAppointmentAction();
                CheckPatientActions();
                appointmentsDataGrid.Items.Clear();
                FillPatientAppointmentTable(LoggedPatient.MedicalRecord.AppointmentIds);
            }
            else
            {
                MessageBox.Show("You must select row first!");
            }
        }

        public bool IsDoctorUpdatedOrAvailable(Doctor doctor, Doctor newDoctor, Appointment appointment,
            DateTime startTimeAppointment, DateTime endTimeAppointment)
        {
            if (newDoctor.Username != doctor.Username)
            {
                if (!IsDoctorAvailable(startTimeAppointment, endTimeAppointment, newDoctor))
                {
                    return false;
                }

                UpdateDoctorAppointments(doctor, appointment.Id);
                doctor = newDoctor;
                doctor.AppointmentIds.Add(appointment.Id);
                return true;
            }
            else
            {
                Console.WriteLine(doctor.Username);
                if (!IsDoctorAvailable(startTimeAppointment, endTimeAppointment, doctor))
                {
                    return false;
                }
            }
            return true;
        }

        public void UpdateAppointment(Appointment appointment, DateTime startTimeAppointment, DateTime endTimeAppointment)
        {
            MainStorage.doctorStorage.Save(MainStorage.Doctors);
            appointment.TimeSlot.StartTime = startTimeAppointment;
            appointment.TimeSlot.EndTime = endTimeAppointment;
            MainStorage.appointmentStorage.Save(MainStorage.Appointments);

        }
        public void UpdatePatientAppointmentAction()
        {
            PatientAppointmentActions patientAppointmentActions =
                new PatientAppointmentActions(LoggedPatient.Username, PatientAction.Update, DateTime.Now);
            MainStorage.PatientAppointmentActions.Add(patientAppointmentActions);
            MainStorage.patientAppointmentActionsStorage.Save(MainStorage.PatientAppointmentActions);
        }
        public void UpdateDoctorAppointments(Doctor doctor, string appId)
        {
            foreach (string id in doctor.AppointmentIds)
            {
                if (id == appId)
                {
                    doctor.AppointmentIds.Remove(appId);
                    break;
                }
            }
        }
        public Doctor FindDoctorByAppointmentId(string id)
        {
            foreach (Doctor doctor in MainStorage.Doctors)
            {
                foreach (string appointmentId in doctor.AppointmentIds)
                {
                    if (appointmentId == id)
                    {
                        return doctor;
                    }
                }
            }
            return null;
        }
        public void EditAppointmentFields(Appointment app)
        {
            dateInput_TextBox.Text = app.TimeSlot.StartTime.Date.ToString("yyyy-MM-dd");
            TimeSpan time = app.TimeSlot.StartTime.TimeOfDay;
            idInput_TextBox.Text = app.Id;
            AppointmentStatus_TextBox.Text = app.AppointmentStatus.ToString();
            AppointmentType_TextBox.Text = app.AppointmentType.ToString();
            // format as a string
            string timeStr = string.Format("{0}", time);
            timeInput_TextBox.Text = timeStr;
            Doctor doctor = FindDoctorByAppointmentId(app.Id);
            string username = doctor.Username;
            selectedDoctore_TextBox.Text = username;
            appointmentsDataGrid.SelectedItem = null;

        }
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            Appointment appointment = row.DataContext as Appointment;
            EditAppointmentFields(appointment);

        }

        public void CheckPatientActions()
        {
            DateTime currentDate = DateTime.Now;
            DateTime thirtyDaysAgo = currentDate.AddDays(-30);

            int counterEditAction = 0;
            int counterDeleteAction = 0;
            int counterCreateAction = 0;

            foreach (PatientAppointmentActions action in MainStorage.PatientAppointmentActions)
            {
                if (LoggedPatient.Username == action.PatientUsername && action.TimeAction > thirtyDaysAgo && action.TimeAction <= DateTime.Now)
                {
                    if (action.PatientAction == PatientAction.Create)
                    {
                        counterCreateAction++;
                    }
                    else if (action.PatientAction == PatientAction.Delete)
                    {
                        counterDeleteAction++;
                    }
                    else if (action.PatientAction == PatientAction.Update)
                    {
                        counterEditAction++;
                    }
                }
            }

            if (IsNeedsToBeBlocked(counterCreateAction, counterEditAction, counterDeleteAction))
            {
                BlockAccount();
            }
        }

        public bool IsNeedsToBeBlocked(int counterCreateAction, int counterEditAction, int counterDeleteAction)
        {
            const int maxCreateActions = 8;
            const int maxEditActions = 5;
            const int maxDeleteActions = 5;

            return counterCreateAction >= maxCreateActions || counterEditAction >= maxEditActions || counterDeleteAction >= maxDeleteActions;
        }

        private void BlockAccount()
        {
            LoggedPatient.Status = Status.Blocked;
            MainStorage.patientStorage.Save(MainStorage.Patients);
            MessageBox.Show("Your account is blocked!");
            new LogInView().Show();
            this.Close();
        }
    }
}
