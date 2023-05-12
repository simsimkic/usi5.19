using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class Schedule
    {
        public string Id { get; set; }
        public List<string> AppointmentIds { get; set; }

        public Schedule() {}

        public Schedule(string id, List<string> appointmentIds)
        {
            this.Id = id;
            this.AppointmentIds = appointmentIds;
        }
    }
}
