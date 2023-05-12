using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class Anamnesis
    {
        public string Observations { get; set; }
        public List<string> Symptoms { get; set; }
        public Anamnesis() {}

        public Anamnesis(string observations, List<string> symptoms)
        {
            this.Observations = observations;
            this.Symptoms = symptoms;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        internal void FromJSON(string v)
        {
            throw new NotImplementedException();
        }
    }
}
