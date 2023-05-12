using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class TransferStorage
    {
        public const string StoragePath = "../../../Data/Transfers.json";

        public Serializer<Transfer> _serializer;


        public TransferStorage()
        {
            _serializer = new Serializer<Transfer>();
        }

        public List<Transfer> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Transfer> transfers)
        {
            _serializer.ToJSON(StoragePath, transfers);
        }
    }
}
