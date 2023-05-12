using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Storage;

namespace ZdravoCorp.Storage
{
    public class MainStorage
    {
        public Person LoggedPerson { get; set; }
        public List<Administrator>? Administrators { get; set; }
        public List<Doctor>? Doctors { get; set; }
        public List<Patient>? Patients { get; set; }
        public List<Nurse>? Nurses { get; set; }
        public List<Hospital>? Hospitals { get; set; }
        public List<Equipment>? Equipments { get; set; }
        public List<Appointment>? Appointments { get; set; }
        public List<FreeDays>? FreeDays { get; set; }
        public List<Medicine>? Medicines { get; set; }
        public List<Notification>? Notifications { get; set; }
        public List<Order>? Orders { get; set; }
        public List<Poll>? Polls { get; set; }
        public List<Renovation>? Renovations { get; set; }
        public List<Transfer>? Transfers { get; set; }

        public List<PatientAppointmentActions>? PatientAppointmentActions { get; set; }

        public AdministratorStorage administratorStorage { get; set; }
        public DoctorStorage doctorStorage { get; set; }
        public PatientStorage patientStorage { get; set; }
        public NurseStorage nurseStorage { get; set; }
        public HospitalStorage hospitalStorage { get; set; }
        public EquipmentStorage equipmentStorage { get; set; }
        public AppointmentStorage appointmentStorage { get; set; }
        public FreeDaysStorage freeDaysStorage { get; set; }
        public MedicineStorage medicineStorage { get; set; }
        public NotificationStorage notificationStorage { get; set; }
        public OrderStorage orderStorage { get; set; }
        public PollStorage pollStorage { get; set; }
        public RenovationStorage renovationStorage { get; set; }
        public TransferStorage transferStorage { get; set; }
        public PatientAppointmentActionsStorage patientAppointmentActionsStorage { get; set; }
        public MainStorage()
        {
            this.Administrators = new List<Administrator>();
            this.Doctors = new List<Doctor>();
            this.Patients = new List<Patient>();
            this.Nurses = new List<Nurse>();
            this.Hospitals = new List<Hospital>();
            this.Equipments = new List<Equipment>();
            this.Appointments = new List<Appointment>();
            this.FreeDays = new List<FreeDays>();
            this.Medicines = new List<Medicine>();
            this.Notifications = new List<Notification>();
            this.Orders = new List<Order>();
            this.Polls = new List<Poll>();
            this.Renovations = new List<Renovation>();
            this.Transfers = new List<Transfer>();
            this.PatientAppointmentActions = new List<PatientAppointmentActions>();
            this.administratorStorage = new AdministratorStorage();
            this.doctorStorage = new DoctorStorage();
            this.patientStorage = new PatientStorage();
            this.nurseStorage = new NurseStorage();
            this.hospitalStorage = new HospitalStorage();
            this.equipmentStorage = new EquipmentStorage();
            this.appointmentStorage = new AppointmentStorage();
            this.freeDaysStorage = new FreeDaysStorage();
            this.medicineStorage = new MedicineStorage();
            this.notificationStorage = new NotificationStorage();
            this.orderStorage = new OrderStorage();
            this.pollStorage = new PollStorage();
            this.renovationStorage = new RenovationStorage();
            this.transferStorage = new TransferStorage();
            this.patientAppointmentActionsStorage = new PatientAppointmentActionsStorage();
        }

    public void loadAllData()
        {
            this.Administrators = administratorStorage.Load();
            this.Doctors = doctorStorage.Load();
            this.Patients = patientStorage.Load();
            this.Nurses = nurseStorage.Load();
            this.Hospitals = hospitalStorage.Load();
            this.Equipments = equipmentStorage.Load();
            this.Appointments = appointmentStorage.Load();
            this.FreeDays = freeDaysStorage.Load();
            this.Medicines = medicineStorage.Load();
            this.Notifications = notificationStorage.Load();
            this.Orders = orderStorage.Load();
            this.Polls = pollStorage.Load();
            this.Renovations = renovationStorage.Load();
            this.Transfers = transferStorage.Load();
            this.PatientAppointmentActions = patientAppointmentActionsStorage.Load();
        }

    }
}
