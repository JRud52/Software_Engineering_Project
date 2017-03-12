using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Software_Engineering_Project.Models
{
    public class Bookings
    {
        public int id { get; set; }
        public int userID { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int roomID { get; set; }
    }
}