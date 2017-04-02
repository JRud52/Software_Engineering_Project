using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Software_Engineering_Project.Models
{
    public class AdminDashModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public int privilage { get; set; }
        public List<Users> users{ get; set; }
        public List<Rooms> rooms { get; set; }
        public Users editUser { get; set; }
        public Users editUserNew { get; set; }
    }
}