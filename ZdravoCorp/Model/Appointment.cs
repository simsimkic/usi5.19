using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public enum AppointmentStatus
    {
        Scheduled,
        Canceled,
        Finished

    }

    public enum AppointmentType
    {
        Appointment,
        Surgery,
        EmergencyAppointment,
        EmergencySurgery
    }
    public class Appointment 
    {
        public string Id { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public Anamnesis Anamnesis { get; set; }
        public TimeSlot TimeSlot { get; set; }

        public string RoomId { get; set; }
        
        public Appointment() {  }

        public Appointment(string id, AppointmentType type, AppointmentStatus status,string roomId, Anamnesis anamnesis, TimeSlot timeSlot)
        {
            this.Id = id;
            this.AppointmentType = type;
            this.AppointmentStatus = status;
            this.RoomId =roomId;
            this.TimeSlot = timeSlot;
            this.Anamnesis = anamnesis;
            
        }


    }
}
