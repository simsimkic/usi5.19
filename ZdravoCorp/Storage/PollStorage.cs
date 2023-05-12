using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class PollStorage
    {
        public const string StoragePath = "../../../Data/Polls.json";

        public Serializer<Poll> _serializer;


        public PollStorage()
        {
            _serializer = new Serializer<Poll>();
        }

        public List<Poll> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Poll> polls)
        {
            _serializer.ToJSON(StoragePath, polls);
        }
    }
}
