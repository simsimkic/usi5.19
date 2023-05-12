using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class Therapy
    {
        public string Description { get; set; }
        public List<string> Medicines { get; set; }
        public Therapy() { }
        public Therapy(string description, List<string> medicines)
        {
            this.Description = description;
            this.Medicines = medicines;
        }

    }
}
