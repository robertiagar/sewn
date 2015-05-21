using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sewn.Models
{
    public class FindPhonesModel
    {
        public IList<ContactModel> Contacts { get; set; }
    }

    public class ContactModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public IList<string> PhoneNumbers { get; set; }
    }
}