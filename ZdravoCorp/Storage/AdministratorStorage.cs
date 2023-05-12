using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class AdministratorStorage
    {
        private const string StoragePath = "../../../Data/Administrator.json";

        private Serializer<Administrator> _serializer;


        public AdministratorStorage()
        {
            _serializer = new Serializer<Administrator>();
        }

        public List<Administrator> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Administrator> administrators)
        {
            _serializer.ToJSON(StoragePath, administrators);
        }
    }
}
