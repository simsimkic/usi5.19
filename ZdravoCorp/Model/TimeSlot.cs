using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class TimeSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public TimeSlot() { }

        public TimeSlot(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public string DateToString() { return ""; }
        public TimeSpan CalculateTime() {
            TimeSpan timeSpan = EndTime - StartTime;

            return timeSpan;
        }

        
    }
}
