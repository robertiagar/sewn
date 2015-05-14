using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sewn.Models
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public DateTime? Date { get; set; }

        public Location()
        {
            Date = null;
        }
    }
}