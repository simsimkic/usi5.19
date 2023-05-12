using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class PatientHistory
    {
        public Appointment Appointment { get; set; }
        public Doctor Doctor { get; set; }

        public PatientHistory() { }

        public PatientHistory(Appointment appointment, Doctor doctor)
        {
            this.Appointment = appointment; 
            this.Doctor = doctor;
        }


    }
}
