using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Software_Engineering_Project.Models
{
    public class Calendar
    {
        public DateTime date { get; set; }
        public List<Bookings> bookings { get; set; }
        public Rooms room { get; set; }
    }
}