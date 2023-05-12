using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public enum RequestStatus
    {
        Accepted,
        InProcess,
        Rejected
    }

    public class FreeDays
    {
        public string Id { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public string Description { get; set; }
        public TimeSlot TimeSlot { get; set; }

        public FreeDays() { }

        public FreeDays(string id, RequestStatus statusRequest, string description, TimeSlot timeSlot)
        {
            this.Id = id;
            this.Description = description;
            this.RequestStatus = statusRequest;
            this.TimeSlot = timeSlot;
            
        }

        
    }
}
