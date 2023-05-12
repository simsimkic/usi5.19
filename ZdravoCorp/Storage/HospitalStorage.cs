using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class HospitalStorage
    {

        private const string StoragePath = "../../../Data/Hospital.json";

        private Serializer<Hospital> _serializer;

        public HospitalStorage()
        {
            _serializer = new Serializer<Hospital>();
        }

        public List<Hospital> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Hospital> hospitals)
        {
            _serializer.ToJSON(StoragePath, hospitals);
        }

    }
}
