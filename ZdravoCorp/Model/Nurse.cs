using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Model
{
    public class Nurse : Person
    {

        public Nurse() { }
        public Nurse(Person person)
        {
            this.FirstName = person.FirstName;
            this.LastName = person.LastName;
            this.Username = person.Username;
            this.Password = person.Password;
            this.Status = person.Status;
        }

       
    }
}
