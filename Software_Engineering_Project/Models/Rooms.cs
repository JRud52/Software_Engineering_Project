using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Software_Engineering_Project.Models
{
    public class Rooms
    {
        public int id { get; set; }
        public int capacity { get; set; }
        public IEnumerable<string> descriptors { get; set; }
        public string descriptor { get; set; }
        public Rooms() {
            capacity = 0;
            descriptors = new List<String>() { "Bio Lab", "Computer Lab", "Regular Classroom" };
        }
    }
}
