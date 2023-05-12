using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class Visit
    {
        public string Id { get; set; }
        public string BloodPressure { get; set; }
        public double Temperature { get; set; }
        public string Observations { get; set; }

        public Visit() {}
        public Visit(string id, string bloodPressure, double temperature, string observation)
        {
            this.Id = id;
            this.BloodPressure = bloodPressure;
            this.Temperature = temperature;
            this.Observations = observation;
        }

    }
}
