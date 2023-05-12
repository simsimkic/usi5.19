using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ZdravoCorp.Model;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Storage
{
    public class OrderStorage
    {
        public const string StoragePath = "../../../Data/Orders.json";

        public Serializer<Order> _serializer;


        public OrderStorage()
        {
            _serializer = new Serializer<Order>();
        }

        public List<Order> Load()
        {
            return _serializer.FromJSON(StoragePath);
        }

        public void Save(List<Order> orders)
        {
            _serializer.ToJSON(StoragePath, orders);
        }
    }
}
