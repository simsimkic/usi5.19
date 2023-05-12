using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public enum PollType
    {
        Hospital,
        Doctor
    }
    public class Poll 
    {
        public string Id { get; set; }
        public string PatientUsername { get; set; }
        public PollType PollType { get; set; }
        public List<Question> Questions { get; set; }
        public Poll() { }

        public Poll(string id, string patientUsername, PollType pollType, List<Question> questions)
        {
            this.Id = id;
            this.PatientUsername = patientUsername;
            this.PollType = pollType;
            this.Questions = questions;
        }

       
    }

}
