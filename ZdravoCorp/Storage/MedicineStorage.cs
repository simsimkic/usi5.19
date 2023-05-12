using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class MedicineStorage
    {
        public const string StoragePath = "../../../Data/Medicines.json";

        public Serializer<Medicine> _serializer;


        public MedicineStorage()
        {
            _serializer = new Serializer<Medicine>();
        }

        public List<Medicine> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Medicine> medicines)
        {
            _serializer.ToJSON(StoragePath, medicines);
        }
    }
}
