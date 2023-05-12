using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Serializer;

namespace ZdravoCorp.Model
{
    public class Administrator : Person
    {
        public Administrator()
        {
            
        } 
        public Administrator(Person person)
        {
            this.FirstName = person.FirstName;
            this.LastName = person.LastName;
            this.Username = person.Username;
            this.Password = person.Password;
            this.Status = person.Status;
        }

        public string[] ToJSON()
        {
            string[] jsonValues = { FirstName, LastName, Username, Password, Status.ToString()};
            return jsonValues;
        }

        public void FromJSON(string[] values)
        {
            FirstName = values[0];
            LastName = values[1];
            Username = values[2];
            Password = values[3];
            Status = (Status)Enum.Parse(typeof(Status), values[4]);
        }
    }
}
