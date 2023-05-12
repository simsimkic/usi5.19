using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class MedicalRecord
    {
        public int Height { get; set; } 
        public int Weight { get; set; } 
        public List<String> MedicalHistory { get; set; }
        public List<String> Allergies { get; set; }
        public List<string> AppointmentIds { get; set; }

        public List<Prescription> Prescriptions { get; set; }

        public Referral Referral { get; set; }

        public HospitalCare HospitalCare { get; set; }
       

        public MedicalRecord() {
            this.MedicalHistory = new List<String>();
            this.Allergies = new List<String>();
            this.AppointmentIds = new List<String>();
            this.Prescriptions = new List<Prescription>();
        }

        public MedicalRecord(int height, int weight, List<String> medicalHistory, List<String> allergies, List<String> appointmentIds, List<Prescription> prescriptions, Referral referral, HospitalCare hospitalCare)
        {
            this.Height = height;
            this.Weight = weight;
            this.MedicalHistory = medicalHistory;
            this.Allergies = allergies;
            this.AppointmentIds = appointmentIds;
            this.Prescriptions = prescriptions;
            this.Referral = referral;
            this.HospitalCare = hospitalCare;
            
        }

        
    }

}
