using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class NurseStorage
    {
        private const string StoragePath = "../../../Data/Nurses.json";

        private Serializer<Nurse> _serializer;


        public NurseStorage()
        {
            _serializer = new Serializer<Nurse>();
        }

        public List<Nurse> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Nurse> nurses)
        {
            _serializer.ToJSON(StoragePath, nurses);
        }
    }
}
