using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class Medicine 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Ingredients { get; set; }

        public Medicine() { }
        public Medicine(string id, string name, List<string> ingredients)
        {
            this.Id = id;
            this.Name = name;
            this.Ingredients = ingredients;
        }

        
    }
}
