using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{

    public enum PatientAction
    {
        Create, 
        Delete, 
        Update
    }
    public class PatientAppointmentActions
    {

        public string PatientUsername { get; set; }
        public PatientAction PatientAction { get; set; }
        public DateTime TimeAction { get; set; }

        public PatientAppointmentActions() { }
        public PatientAppointmentActions(string patientUsername, PatientAction patientAction, DateTime timeAction)
        {
            PatientUsername = patientUsername;
            PatientAction = patientAction;
            TimeAction = timeAction;
        }
    }
}
