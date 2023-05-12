using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public enum RenovationType
    {
        Separation,
        Merging,
        None
    }
    public class Renovation
    {
        public string Id { get; set; }
        public string RoomId { get; set; }
        public  RenovationType RenovationType { get; set;}
        public TimeSlot TimeSlot { get; set; }

        public Renovation() { }

        public Renovation(string id, string roomId, RenovationType renovationType, TimeSlot timeSlot)
        {
            this.Id = id;
            this.RoomId = roomId;
            this.TimeSlot = timeSlot;
            this.RenovationType = renovationType;
        }

    }
}
