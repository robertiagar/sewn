using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sewn.Models
{
    public class Friend
    {
        public string Id { get; set; }
        public Status Status { get; set; }

        public Friend()
        {
            Status = Status.PendingResponse;
        }
    }

    public enum Status
    {
        PendingResponse,
        Accepted
    }
}