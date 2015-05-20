using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sewn.Models
{
    public class FindPhoneViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public IList<string> PhoneNumbers { get; set; }
    }
}