using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Software_Engineering_Project.Models
{
    public class RoomSearchModel
    {       
        public List<Models.Rooms> rooms { get; set; }
        public RoomSearchModel()
        {
            rooms = new List<Models.Rooms>();
        }
    }
}