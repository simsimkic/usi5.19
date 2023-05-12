using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class FreeDaysStorage
    {
        public const string StoragePath = "../../../Data/FreeDays.json";

        public Serializer<FreeDays> _serializer;


        public FreeDaysStorage()
        {
            _serializer = new Serializer<FreeDays>();
        }

        public List<FreeDays> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<FreeDays> freeDays)
        {
            _serializer.ToJSON(StoragePath, freeDays);
        }
    }
}
