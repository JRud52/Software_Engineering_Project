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
            ViewData["date"] = System.DateTime.Now;
            return View();
        }
        

        public ActionResult ViewBookings()
        {
            ViewData["date"] = System.DateTime.Now;

            return View();
        }

        public ActionResult BookRoom()
        {
            //ViewBag.Message = "Your contact page.";
                      
            return View();
        }

    }
}