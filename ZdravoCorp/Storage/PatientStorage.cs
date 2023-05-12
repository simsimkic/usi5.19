using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class PatientStorage
    {
        private const string StoragePath = "../../../Data/Patients.json";

        private Serializer<Patient> _serializer;


        public PatientStorage()
        {
            _serializer = new Serializer<Patient>();
        }

        public List<Patient> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Patient> patients)
        {
            _serializer.ToJSON(StoragePath, patients);
        }
    }
}
