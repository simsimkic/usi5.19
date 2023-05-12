using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public enum NotificationStatus
    {
        NotShowed,
        Showed
    }
    public class Notification
    {
        public string Id { get; set; }
        public string PersonUsername { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public NotificationStatus NotificationStatus { get; set; }
        public Notification() { }

        public Notification(string id, string personUsername, string title, string description)
        {
            this.Id = id;
            this.PersonUsername = personUsername;
            this.Title = title;
            this.Description = description;
            this.NotificationStatus = NotificationStatus.NotShowed;
        }

       
    }
}
