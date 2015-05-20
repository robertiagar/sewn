using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sewn.Models
{
    public class Friendship
    {
        public virtual ApplicationUser Requester { get; set; }
        public virtual ApplicationUser Accepter { get; set; }
        public virtual string RequesterId { get; set; }
        public virtual string AccepterId { get; set; }
        public DateTime Added { get; set; }
        public DateTime Updated { get; set; }
        public Status Status { get; set; }

        public Friendship()
        {
            Added = DateTime.Now;
            Updated = Added;
            Status = Status.PendingResponse;
        }
    }

    public enum Status
    {
        Accepted,
        PendingResponse,
        Blocked,
        Spam
    }
}