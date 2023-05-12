using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZdravoCorp.Commands;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;
using ZdravoCorp.View;

namespace ZdravoCorp.ViewModel
{
    public class CreateEmergencyAppointmentViewModel : ViewModelBase
    {
        private MainStorage mainStorage { get; set; }
        private CreateEmergencyAppointmentView createEmergencyAppointmentView { get; set; }
        public ICommand BackCommand { get; }
        public ICommand GetEmergencyAppointmentDataCommand { get; }
        public ICommand CreateEmergencyAppointmentCommand { get; }
        public ICommand InsertSurgeryDurationCommand { get; }
        public ICommand AppointmentCommand { get; }
        public Appointment emergencyAppointment { get; set; }
        public int emergencyAppointmentDuration { get; set; }
        public Doctor choosenDoctor { get; set; }
        public DateTime emergencyAppointmentDate { get; set; }
        public Dictionary<string, string> potentialTakenTerms { get; set; }
        public Dictionary<Doctor, List<Appointment>> appointmentsForEachDoctor { get; set; }
        public Dictionary<Doctor, DateTime> earliestTermForEachDoctor { get; set; }
        public Appointment postponedAppointment { get; set; }
        public AppointmentType appointmentType { get; set; }
        public bool isAllTermsTaken { get; set; }


        public List<Patient> PatientsTable { get; set; }
        public Patient SelectedPatient { get; set; }

        public string SelectedSpecialization { get; set; }
        private List<string> specializationTable;
        public List<string> SpecializationTable
        {
            get { return specializationTable; }
            set
            {
                specializationTable = value;
                OnPropertyChanged("SpecializationTable");
            }
        }

        private string _searchPatient;
        public string SearchPatient
        {
            get { return _searchPatient; }
            set
            {
                _searchPatient = value;
                OnPropertyChanged(nameof(SearchPatient));
                PerformUpdatePatients(_searchPatient);
            }
        }

        private string _searchSpecialization;
        public string SearchSpecialization
        {
            get { return _searchSpecialization; }
            set
            {
                _searchSpecialization = value;
                OnPropertyChanged(nameof(SearchSpecialization));
                PerformUpdateSpecializations(_searchSpecialization); 
            }
        }


        private string _specializedDoctor;
        public string SpecializedDoctor
        {
            get { return _specializedDoctor; }
            set
            {
                _specializedDoctor = value;
                OnPropertyChanged(nameof(SpecializedDoctor));
            }
        }

        private string _startDate;
        public string StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        private string _startTime;
        public string StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }

        private ObservableCollection<string> _terms = new ObservableCollection<string>();
        public ObservableCollection<string> Terms
        {
            get { return _terms; }
            set { _terms = value; OnPropertyChanged(); }
        }

        private string _selectedTerm;
        public string SelectedTerm
        {
            get { return _selectedTerm; }
            set
            {
                if (_selectedTerm != value)
                {
                    _selectedTerm = value;
                    OnPropertyChanged(nameof(SelectedTerm));
                }
            }
        }

        private bool _isAppointmentSelected;
        public bool IsAppointmentSelected
        {
            get { return _isAppointmentSelected; }
            set
            {
                _isAppointmentSelected = value;
            }
        }

        private bool _isSurgerySelected;
        public bool IsSurgerySelected
        {
            get { return _isSurgerySelected; }
            set
            {
                _isSurgerySelected = value;
            }
        }

        private string _surgeryDuration;
        public string SurgeryDuration
        {
            get { return _surgeryDuration; }
            set
            {
                _surgeryDuration = value;
                OnPropertyChanged(nameof(SurgeryDuration));
            }
        }

        private Visibility _createEmergencyAppointmentVisibility;
        public Visibility CreateEmergencyAppointmentVisibility
        {
            get { return _createEmergencyAppointmentVisibility; }
            set
            {
                _createEmergencyAppointmentVisibility = value;
                OnPropertyChanged(nameof(CreateEmergencyAppointmentVisibility));
            }
        }

        private Visibility _getDataVisibility;
        public Visibility GetDataVisibility
        {
            get { return _getDataVisibility; }
            set
            {
                _getDataVisibility = value;
                OnPropertyChanged(nameof(GetDataVisibility));
            }
        }

        private Visibility _surgeryDurationVisibility;
        public Visibility SurgeryDurationVisibility
        {
            get { return _surgeryDurationVisibility; }
            set
            {
                _surgeryDurationVisibility = value;
                OnPropertyChanged(nameof(SurgeryDurationVisibility));
            }
        }

        


        public CreateEmergencyAppointmentViewModel(MainStorage mainStorage, CreateEmergencyAppointmentView createEmergencyAppointmentView)
        {
            this.mainStorage = mainStorage;
            this.createEmergencyAppointmentView = createEmergencyAppointmentView;
            this.PatientsTable = mainStorage.Patients;
            this.SpecializationTable = getAllSpecializations();
            this.emergencyAppointment = new Appointment();
            this.postponedAppointment = new Appointment();
            this.emergencyAppointmentDuration = 0;
            this.appointmentsForEachDoctor = new Dictionary<Doctor, List<Appointment>>();
            this.potentialTakenTerms = new Dictionary<string, string>();
            this.earliestTermForEachDoctor = new Dictionary<Doctor, DateTime>();
            this.isAllTermsTaken = false;
            SurgeryDurationVisibility = Visibility.Collapsed;

            BackCommand = new RelayCommand(Back);
            GetEmergencyAppointmentDataCommand = new RelayCommand(GetEmergencyAppointmentData);
            CreateEmergencyAppointmentCommand = new RelayCommand(CreateEmergencyAppointment);
            InsertSurgeryDurationCommand = new RelayCommand(SetSurgeryDurationTextBoxVisible);
            AppointmentCommand = new RelayCommand(SetSurgeryDurationTextBoxHiden);
        }

        public void SetSurgeryDurationTextBoxVisible(object parameter)
        {
            SurgeryDurationVisibility = Visibility.Visible;
        }

        public void SetSurgeryDurationTextBoxHiden(object parameter)
        {
            SurgeryDurationVisibility = Visibility.Hidden;
        }


        public List<string> getAllSpecializations()
        {
            List<string> specializations = new List<string>();
            foreach (Doctor doctor in mainStorage.Doctors)
            {
                if (!specializations.Contains(doctor.Specialization.ToString()))
                {
                    specializations.Add(doctor.Specialization.ToString());
                }
            }

            return specializations;
        }

        private void PerformUpdatePatients(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                PatientsTable = mainStorage.Patients.ToList();
            }
            else
            {
                this.PatientsTable = mainStorage.Patients.Where(patient =>
                patient.FirstName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                || patient.LastName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                || patient.Username.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                ).ToList();
            }

            OnPropertyChanged(nameof(this.PatientsTable));


        }
        private void PerformUpdateSpecializations(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                SpecializationTable = getAllSpecializations();
            }
            else
            {
                this.SpecializationTable = SpecializationTable.Where(specialization =>
                specialization.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)
                ).ToList();
            }
            OnPropertyChanged(nameof(this.SpecializationTable));
        }

        public void GetEmergencyAppointmentData(object parameter)
        {
            DateTime currentTime = DateTime.Now;
            DateTime twoHoursFromNow = currentTime.AddHours(2);
            bool isDurationTimeValid = true;

            if (_isSurgerySelected)
            {
                isDurationTimeValid  = ValidateSurgeryDurationInput(_surgeryDuration);
                if (!isDurationTimeValid)
                {
                    MessageBox.Show($"Check again! Surgery duration is not valid!");
                }

            }
            if(isDurationTimeValid || _isAppointmentSelected)
            {
                CheckEmergencyDuration();
                FilterAppointmentsBySpecializationAndTimeRange();
                FindDoctorsWithoutAppointments(appointmentsForEachDoctor, currentTime, twoHoursFromNow, emergencyAppointmentDuration);
                SortAppointmentTermsForEachDoctor(appointmentsForEachDoctor, currentTime, twoHoursFromNow, emergencyAppointmentDuration);

                if (choosenDoctor != null && emergencyAppointmentDate != DateTime.MinValue)
                {
                    SpecializedDoctor = choosenDoctor.Username;
                    StartDate = emergencyAppointmentDate.Date.ToString("yyyy-MM-dd");
                    TimeSpan timeOfDay = emergencyAppointmentDate.TimeOfDay;
                    DateTime dateTime = DateTime.MinValue + timeOfDay;
                    StartTime = dateTime.ToString(@"HH\:mm\:ss");
                }
                else
                {
                    isAllTermsTaken = true;
                    potentialTakenTerms = SortPotentialTakenTerms(appointmentsForEachDoctor);
                    foreach (string appointmentId in potentialTakenTerms.Keys)
                    {
                        Console.WriteLine(potentialTakenTerms[appointmentId]);
                        Terms.Add(potentialTakenTerms[appointmentId]);  
                    }

                    
                }
            }

            foreach(Patient patient in PatientsTable)
            {

            }
            
        }

        public void CreateEmergencyAppointment(object parameter)
        {
            if (_selectedTerm != null)
            {
                string[] selectedTermInforamtions = _selectedTerm.Split(";");
                SpecializedDoctor = selectedTermInforamtions[0].Trim();
                string[] dateTime = selectedTermInforamtions[1].Split('T');
                StartDate = dateTime[0];
                StartTime = dateTime[1];
            }

            foreach(string appointmentId in potentialTakenTerms.Keys)
            {
                if(_selectedTerm == potentialTakenTerms[appointmentId])
                {
                    postponedAppointment.Id = appointmentId;
                }
            }

            string dateAndTime;

            string doctorUsername = SpecializedDoctor;
            dateAndTime = StartDate + "T" + StartTime;

            foreach (Doctor doctor in mainStorage.Doctors)
            {
                if (doctorUsername.Equals(doctor.Username))
                {
                    choosenDoctor = doctor;
                    break;
                }
            }
            DateTime startTime = DateTime.ParseExact(dateAndTime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endTime = startTime.AddMinutes(emergencyAppointmentDuration);
            emergencyAppointment.Id = GenerateNewAppointmentId();
            emergencyAppointment.AppointmentType = appointmentType;
            emergencyAppointment.AppointmentStatus = AppointmentStatus.Scheduled;
            TimeSlot timeSlot = new TimeSlot();
            timeSlot.StartTime = startTime;
            timeSlot.EndTime = endTime;
            emergencyAppointment.TimeSlot = timeSlot;


            mainStorage.Appointments.Add(emergencyAppointment);
            mainStorage.appointmentStorage.Save(mainStorage.Appointments);

            foreach(Patient patient in mainStorage.Patients)
            {
                if (patient.Username.Equals(SelectedPatient.Username))
                {
                    patient.MedicalRecord.AppointmentIds.Add(emergencyAppointment.Id);
                    break;
                }
            }
            mainStorage.patientStorage.Save(mainStorage.Patients);

            foreach(Doctor doctor in mainStorage.Doctors)
            {
                if(doctor.Username.Equals(SpecializedDoctor))
                {
                    doctor.AppointmentIds.Add(emergencyAppointment.Id);
                    break;
                }
            }
            mainStorage.doctorStorage.Save(mainStorage.Doctors);
            mainStorage.doctorStorage.Save(mainStorage.Doctors);
            if (isAllTermsTaken)
            {
                PostponeChoosenAppointment();

            }

            MessageBoxResult result =  MessageBox.Show($"Successfully scheduled emergency appointment! \nReview information: \nAppointment Id: "
                    + emergencyAppointment.Id + "\nDoctor: " + SpecializedDoctor + "\nPatient: " + SelectedPatient.Username
                    + "\nStart Date: " + StartDate + "\nStart Time: " + StartTime);

            if (result == MessageBoxResult.OK)
            {
                CreateEmergencyAppointmentVisibility = Visibility.Hidden;
                GetDataVisibility = Visibility.Collapsed;

            }
        }

        public void CreateDoctorNotificationAboutDelay()
        {

            Notification doctorNotification = new Notification();
            string notificationId;
            if(mainStorage.Notifications.Count == 0)
            {
                notificationId = "notification1";
            }
            else
            {
                notificationId = GenerateNewNotificationId();
            }
            doctorNotification.Id = notificationId;
            doctorNotification.PersonUsername= SpecializedDoctor;
            doctorNotification.Title = "ODLOZEN TERMIN PREGLEDA";
            doctorNotification.Description = "Odlozen pregled: " + postponedAppointment.Id + "." + " Hitan pregled u terminu: " + StartDate + " " + StartTime;
            mainStorage.Notifications.Add(doctorNotification);
            mainStorage.notificationStorage.Save(mainStorage.Notifications);

        }

        public void CreatePatientNotificationAboutDelay()
        {
            Notification patientNotification = new Notification();
            string notificationId;
            if (mainStorage.Notifications.Count == 0)
            {
                notificationId = "notification1";
            }
            else
            {
                notificationId = GenerateNewNotificationId();
            }
            patientNotification.Id = notificationId;
            patientNotification.PersonUsername = SelectedPatient.Username;
            patientNotification.Title = "HITAN PREGLED";
            patientNotification.Description = "Hitan pregled u terminu: " + StartDate + " " + StartTime;
            mainStorage.Notifications.Add(patientNotification);
            mainStorage.notificationStorage.Save(mainStorage.Notifications);
        }

        public string GenerateNewAppointmentId()
        {
            List<int> appointmentIds = new List<int>();
            foreach (Appointment appointment in mainStorage.Appointments)
            {
                Match match = Regex.Match(appointment.Id, @"\d+$");
                int number = Int32.Parse(match.Value);
                appointmentIds.Add(number);
            }

            int newIdNumber = appointmentIds.Max() + 1;
            string newAppointmentId = "appointment" + newIdNumber;
            return newAppointmentId;
        }

        public string GenerateNewNotificationId()
        {
            List<int> notificationIds = new List<int>();
            foreach (Notification notification in mainStorage.Notifications)
            {
                Match match = Regex.Match(notification.Id, @"\d+$");
                int number = Int32.Parse(match.Value);
                notificationIds.Add(number);
            }

            int newIdNumber = notificationIds.Max() + 1;
            string newNotificationId = "notification" + newIdNumber;
            return newNotificationId;
        }

        public void PostponeChoosenAppointment()
        {
            foreach (Appointment appointment in mainStorage.Appointments)
            {
                if (appointment.Id == postponedAppointment.Id)
                {
                    appointment.AppointmentStatus = AppointmentStatus.Canceled;
                    break;
                }
            }
            mainStorage.appointmentStorage.Save(mainStorage.Appointments);

            CreateDoctorNotificationAboutDelay();
            CreatePatientNotificationAboutDelay();
        }

        public void CheckEmergencyDuration()
        {
            if (_isAppointmentSelected)
            {
                emergencyAppointmentDuration = 15;
                appointmentType = AppointmentType.EmergencyAppointment;
                
            }
            else if (_isSurgerySelected)
            {
                emergencyAppointmentDuration = int.Parse(_surgeryDuration)*60;
                appointmentType = AppointmentType.EmergencyAppointment;

            }
        }

        public void FilterAppointmentsBySpecializationAndTimeRange()
        {
            DateTime currentTime = DateTime.Now;
            DateTime twoHoursFromNow = currentTime.AddHours(2);
            List<Doctor> specializedDoctors = new List<Doctor>();

            foreach(Doctor doctor in mainStorage.Doctors)
            {
                if (doctor.Specialization.ToString().Equals(SelectedSpecialization))
                {
                    specializedDoctors.Add(doctor);
                }
            }

            foreach(Doctor specializedDoctor in specializedDoctors)
            {
                List<Appointment> appointmentsOnDay = new List<Appointment>();

                foreach (Appointment appointment in mainStorage.Appointments)
                {
                    foreach (string appointmentId in specializedDoctor.AppointmentIds)
                    {
                        if (appointment.Id == appointmentId && appointment.AppointmentStatus == AppointmentStatus.Scheduled)
                        {
                            bool isStartingWithinTwoHours = appointment.TimeSlot.StartTime >= currentTime && appointment.TimeSlot.StartTime <= twoHoursFromNow;
                            bool isEndingWithinTwoHours = appointment.TimeSlot.EndTime >= currentTime && appointment.TimeSlot.EndTime <= twoHoursFromNow;
                            bool isStartingAndEndingWithinTwoHours = appointment.TimeSlot.StartTime >= currentTime && appointment.TimeSlot.EndTime <= twoHoursFromNow;
                            bool isStartingEarlierAndEndingAfter = appointment.TimeSlot.StartTime <= currentTime && appointment.TimeSlot.EndTime >= twoHoursFromNow;

                            if (isStartingWithinTwoHours || isEndingWithinTwoHours || isStartingAndEndingWithinTwoHours || isStartingEarlierAndEndingAfter)
                            {
                                appointmentsOnDay.Add(appointment);
                                break;
                            }
                        }
                    }
                }
                if (!appointmentsForEachDoctor.ContainsKey(specializedDoctor))
                {
                    appointmentsForEachDoctor.Add(specializedDoctor, appointmentsOnDay);
                }
                
            }
        }

        public void FindDoctorsWithoutAppointments(Dictionary<Doctor, List<Appointment>> appointmentsForEachDoctor, DateTime currentTime, DateTime twoHoursFromNow, int emergencyAppointmentDuration)
        {
            foreach (Doctor doctor in appointmentsForEachDoctor.Keys)
            {
                if (appointmentsForEachDoctor[doctor].Count == 0)
                {
                    choosenDoctor = doctor;
                    emergencyAppointmentDate = currentTime;
                    return;
                }
            }
        }

        public void SortAppointmentTermsForEachDoctor(Dictionary<Doctor, List<Appointment>> appointmentsForEachDoctor, DateTime currentTime, DateTime twoHoursFromNow, int emergencyAppointmentDuration)
        {
            foreach (Doctor doctor in appointmentsForEachDoctor.Keys)
            {
               if (appointmentsForEachDoctor[doctor].Count == 1)
               {
                    GetAppointmentTermForSingleAppointmentDoctor(appointmentsForEachDoctor[doctor], doctor, currentTime, twoHoursFromNow, emergencyAppointmentDuration);
               }
               else
               {
                    appointmentsForEachDoctor[doctor].Sort((a1, a2) => a1.TimeSlot.StartTime.CompareTo(a2.TimeSlot.StartTime));
                    GetAppointmentTermForEachDoctor(appointmentsForEachDoctor[doctor], doctor, currentTime, twoHoursFromNow, emergencyAppointmentDuration);
               }
            }
            if(earliestTermForEachDoctor.Count != 0) {
                FindDoctorWithEarliestTerm(earliestTermForEachDoctor);
            }
            
        }

        public void GetAppointmentTermForEachDoctor(List<Appointment> appointments, Doctor doctor, DateTime currentTime, DateTime twoHoursFromNow, int emergencyAppointmentDuration)
        {
            for (int i = 0; i < appointments.Count - 1; i++)
            {
                Appointment currentAppointment = appointments[i];
                Appointment nextAppointment = appointments[i + 1];

                DateTime currentEndTime = currentAppointment.TimeSlot.EndTime;
                DateTime currentStartTime = currentAppointment.TimeSlot.StartTime;
                DateTime nextStartTime = nextAppointment.TimeSlot.StartTime;
                TimeSpan timeDifference = nextStartTime - currentEndTime;
                TimeSpan startAndFirstTermDifference = currentStartTime - currentTime;
                Console.WriteLine(timeDifference.TotalMinutes);

                if (startAndFirstTermDifference.TotalMinutes >= emergencyAppointmentDuration)
                {
                    CalculatePotentialAppointmentTerm(currentTime, doctor);
                    return;
                }
                if (timeDifference.TotalMinutes >= emergencyAppointmentDuration)
                {
                    CalculatePotentialAppointmentTerm(currentEndTime, doctor);
                    return;
                }
                else if (nextAppointment == appointmentsForEachDoctor[doctor][appointmentsForEachDoctor[doctor].Count - 1])
                {
                    TimeSpan restTime = twoHoursFromNow - nextAppointment.TimeSlot.EndTime;
                    if (restTime.TotalMinutes >= emergencyAppointmentDuration)
                    {
                        CalculatePotentialAppointmentTerm(nextAppointment.TimeSlot.EndTime, doctor);
                    }
                }
            }
        }

        public void GetAppointmentTermForSingleAppointmentDoctor(List<Appointment> appointments, Doctor doctor, DateTime currentTime, DateTime twoHoursFromNow, int emergencyAppointmentDuration)
        {
            for (int i = 0; i < appointmentsForEachDoctor[doctor].Count; i++)
            {
                Appointment singleAppointment = appointmentsForEachDoctor[doctor][i];
                TimeSpan startTimeDifference = singleAppointment.TimeSlot.StartTime - currentTime;
                TimeSpan restTime = twoHoursFromNow - singleAppointment.TimeSlot.EndTime;

                if (startTimeDifference.TotalMinutes >= emergencyAppointmentDuration)
                {
                    CalculatePotentialAppointmentTerm(currentTime, doctor);
                }
                else if (restTime.TotalMinutes >= emergencyAppointmentDuration)
                {
                    CalculatePotentialAppointmentTerm(singleAppointment.TimeSlot.EndTime, doctor);
                }

            }
        }
       

        public DateTime CalculatePotentialAppointmentTerm(DateTime option,Doctor doctor)
        {
            DateTime potentialTerm = option;
            earliestTermForEachDoctor.Add(doctor, potentialTerm);
            return potentialTerm;
        }

        public void FindDoctorWithEarliestTerm(Dictionary<Doctor, DateTime> earliestTermForEachDoctor)
        {
            DateTime earliestTerm = earliestTermForEachDoctor.Values.Min();
            foreach(Doctor doctor in earliestTermForEachDoctor.Keys)
            {
                if (earliestTermForEachDoctor[doctor] == earliestTerm)
                {
                    choosenDoctor = doctor;
                    emergencyAppointmentDate = earliestTerm;
                }
            }
        }



        public Dictionary<string, string> SortPotentialTakenTerms(Dictionary<Doctor, List<Appointment>> appointmentsForEachDoctor)
        {
            List<Appointment> appointmentsForAllDoctors = new List<Appointment>();

            foreach(Doctor doctor in appointmentsForEachDoctor.Keys)
            {
                foreach(Appointment appointment in appointmentsForEachDoctor[doctor])
                {
                    appointmentsForAllDoctors.Add(appointment);
                }

            }

            appointmentsForAllDoctors.Sort((a1, a2) => a1.TimeSlot.StartTime.CompareTo(a2.TimeSlot.StartTime));

            foreach(Appointment appointment in appointmentsForAllDoctors)
            {
                Console.WriteLine(appointment.TimeSlot.StartTime);
            }

            if (appointmentsForAllDoctors.Count >= 5)
            {
                var firstFiveItems = appointmentsForAllDoctors.Take(5).ToList();
                ArrangeListBoxPrinting(firstFiveItems);
                
            }
            else
            {
                ArrangeListBoxPrinting(appointmentsForAllDoctors);
            }
            return potentialTakenTerms;
        }

        public void ArrangeListBoxPrinting(List<Appointment> appointmentsForAllDoctors)
        {

            foreach (Appointment appointment in appointmentsForAllDoctors)
            {
                foreach (Doctor doctor in appointmentsForEachDoctor.Keys)
                {
                    string terms = " ";
                    if (appointmentsForEachDoctor[doctor].Contains(appointment))
                    {
                        terms += doctor.Username + ";" + appointment.TimeSlot.StartTime.ToString("yyyy-MM-ddTHH:mm:ss");
                        potentialTakenTerms.Add(appointment.Id, terms);
                    }
                }
            }
            
        }

        public bool ValidateSurgeryDurationInput(string surgeryDurationInput)
        {
            int parsedValue;
            if (int.TryParse(surgeryDurationInput, out parsedValue))
            {
                return parsedValue >= 0 && parsedValue <= 2;
            }
            else if (string.IsNullOrEmpty(surgeryDurationInput))
            {
                return false;
            }
            return false;
        }

        public void Back(object parameter)
        {
            new MenuNurseView(mainStorage).Show();
            this.createEmergencyAppointmentView.Close();
        }

    }
}
