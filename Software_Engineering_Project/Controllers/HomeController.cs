using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Software_Engineering_Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewBookings()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult BookRoom()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult LogOn(Models.Users user)
        {
           // System.Diagnostics.Debug.WriteLine("");
            System.Configuration.Configuration rootWebConfig =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/Software_Engineering_Project");
            System.Configuration.ConnectionStringSettings connString;
            if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
            {
                connString =
                    rootWebConfig.ConnectionStrings.ConnectionStrings["db"];
                if (connString != null)
                    System.Diagnostics.Debug.WriteLine("Connection string = \"{0}\"",
                        connString.ConnectionString);
                else
                    System.Diagnostics.Debug.WriteLine("No connection string");
            }




            // show the passed data in a seperate page.
            return Content("Username " + user.email + " <br/>Password: " + user.hash); //View(user);
        }

        [HttpPost]
        public ActionResult QueryRoom(Models.Rooms room)
        {            
            return Content(""); 
        }
    }
}