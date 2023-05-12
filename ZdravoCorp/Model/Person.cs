using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public enum Status
    {
        Active,
        Blocked

    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Status Status { get; set; }

        public Person() { }

        public Person(string firstName, string lastName, string username, string password, Status status)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Username = username;
            this.Password = password;
            this.Status = status;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName} {Username} {Password} {Status}";
        }
    }
}
