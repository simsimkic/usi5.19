using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class HospitalCare
    {
        public string Id { get; set; }
        public int Duration { get; set; }
        public List<String> AdditionalTests { get; set; }
        public string RoomId { get; set; }
        public Therapy Therapy { get; set; }
        public List<Visit> Visits { get; set; }
        public HospitalCare() { }

        public HospitalCare(string id, int duration, List<string> additionalTests, string roomId, Therapy therapy, List<Visit> visits)
        {
            this.Id = id;
            this.Duration = duration;
            this.AdditionalTests = additionalTests;
            this.RoomId = roomId;
            this.Therapy = therapy;
            this.Visits = visits;
        }
    }
}
