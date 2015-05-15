using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sewn.Models
{
    public class Friendship
    {
        public virtual ApplicationUser User1 { get; set; }
        public virtual ApplicationUser User2 { get; set; }
        public virtual string UserId1 { get; set; }
        public virtual string UserId2 { get; set; }
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