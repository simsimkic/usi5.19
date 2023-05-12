using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;
using ZdravoCorp.Commands;



namespace ZdravoCorp.Storage
{
    public class DoctorStorage
    {
        public const string StoragePath = "../../../Data/Doctors.json";
        public Serializer<Doctor> _serializer;


        public DoctorStorage()
        {
            _serializer = new Serializer<Doctor>();
        }

        public List<Doctor> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Doctor> doctors)
        {
            _serializer.ToJSON(StoragePath, doctors);
        }
    }
}
