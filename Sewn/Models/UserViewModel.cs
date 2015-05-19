using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sewn.Models
{
    public class UserViewModel
    {
        public virtual string Email { get; set; }
        public virtual string Id { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string UserName { get; set; }
        public virtual Location Location { get; set; }
        public virtual IEnumerable<UserViewModel> Friends { get; set; }
    }
}