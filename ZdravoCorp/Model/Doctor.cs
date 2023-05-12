using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Model
{
    public enum Specialization
    {
        GeneralPractitioner,
        Cardiologist,
        Dermatologist,
        Neurologist,
        Pediatrician

    }
    public class Doctor : Person
    {
        
        public Specialization Specialization { get; set; }
        public List<string> AppointmentIds { get; set; }
        public List<string> FreeDaysIds { get; set; }


        public Doctor() { }
        public Doctor(Person person, Specialization specialization, List<string> appointmentIds, List<string> freeDaysIds)
        {
            this.FirstName = person.FirstName;
            this.LastName = person.LastName;
            this.Username = person.Username;
            this.Password = person.Password;
            this.Status = person.Status;
            this.Specialization = specialization;
            this.AppointmentIds = appointmentIds;
            this.FreeDaysIds = freeDaysIds;
        }

        
    }
}
