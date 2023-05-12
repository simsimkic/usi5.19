using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class NotificationStorage
    {
        public const string StoragePath = "../../../Data/Notifications.json";

        public Serializer<Notification> _serializer;


        public NotificationStorage()
        {
            _serializer = new Serializer<Notification>();
        }

        public List<Notification> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Notification> notifications)
        {
            _serializer.ToJSON(StoragePath, notifications);
        }
    }
}
