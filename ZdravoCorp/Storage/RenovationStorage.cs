using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class RenovationStorage
    {
        public const string StoragePath = "../../../Data/Renovations.json";

        public Serializer<Renovation> _serializer;


        public RenovationStorage()
        {
            _serializer = new Serializer<Renovation>();
        }

        public List<Renovation> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Renovation> renovations)
        {
            _serializer.ToJSON(StoragePath, renovations);
        }
    }
}
