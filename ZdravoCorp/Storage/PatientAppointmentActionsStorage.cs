using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class PatientAppointmentActionsStorage
    {
        private const string StoragePath = "../../../Data/PatientAppointmentActions.json";

        private Serializer<PatientAppointmentActions> _serializer;


        public PatientAppointmentActionsStorage()
        {
            _serializer = new Serializer<PatientAppointmentActions>();
        }

        public List<PatientAppointmentActions> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<PatientAppointmentActions> patientsActions)
        {
            _serializer.ToJSON(StoragePath, patientsActions);
        }
    }
}

