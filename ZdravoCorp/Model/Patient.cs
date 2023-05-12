using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Model
{
    public class Patient : Person
    {
        [JsonIgnore]
        public Patient LoggedPatient { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
        
        public Patient() { }

        public Patient(Person person, MedicalRecord medicalRecord)
        {
            this.FirstName = person.FirstName;
            this.LastName = person.LastName;
            this.Username = person.Username;
            this.Password = person.Password;
            this.Status = person.Status;
            this.MedicalRecord = medicalRecord;
            
        }

        public Patient(Patient patient)
        {
            this.LoggedPatient = patient;
        }

       
    }
}
