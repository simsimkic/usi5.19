using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public class Question
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }

        public Question(){ }

        public Question(string id, string text, int rating)
        {
            this.Id = id;
            this.Text = text;
            this.Rating = rating;
        }
    }
}
