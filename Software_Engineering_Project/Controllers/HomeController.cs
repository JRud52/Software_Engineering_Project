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

        public PartialViewResult UpdateCalendar(System.DateTime currentCalendarDate, bool next) {
            
            if (next) 
            {
                currentCalendarDate = currentCalendarDate.AddMonths(1);
            }
            else 
            {
                if (currentCalendarDate.Month == 1) 
                {
                    currentCalendarDate = currentCalendarDate.AddYears(-1);
                    currentCalendarDate = currentCalendarDate.AddMonths(11);
                    System.Diagnostics.Debug.WriteLine("Date = " + currentCalendarDate);
                }
                else 
                { 
                    currentCalendarDate = currentCalendarDate.AddMonths(-1);
                }
            }
            return PartialView("Calendar", new Software_Engineering_Project.Models.Calendar() { date = currentCalendarDate });
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

        

        [HttpPost]
        public ActionResult QueryRoom(Models.Rooms room)
        {            
            return Content(""); 
        }
    }
}