using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class EquipmentStorage
    {
        private const string StoragePath = "../../../Data/Equipments.json";

        private Serializer<Equipment> _serializer;

        public EquipmentStorage()
        {
            _serializer = new Serializer<Equipment>();
        }

        public List<Equipment> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Equipment> equipments)
        {
            _serializer.ToJSON(StoragePath, equipments);
        }
    }
}
