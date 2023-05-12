using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class AppointmentStorage
    {
        public const string StoragePath = "../../../Data/Appointments.json";

        public Serializer<Appointment> _serializer;


        public AppointmentStorage()
        {
            _serializer = new Serializer<Appointment>();
        }

        public List<Appointment> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Appointment> appointments)
        {
            _serializer.ToJSON(StoragePath, appointments);
        }
    }
}
