using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public enum ReferralType
    {
        Appointment, 
        HospitalCare
    }
    public class Referral
    {
        public ReferralType Type { get; set; }
        public string Note { get; set; }
        public Referral() { }
        public Referral(ReferralType type, string note) { 
            this.Type = type;
            this.Note = note;
        }
    }
}
