using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Software_Engineering_Project.Controllers
{
    public class BookingController : Controller
    {
        // GET: Booking
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BookRoom()
        {
            //            return View("BookRoom", new Models.Calendar() { date = System.DateTime.Now });
            return View("BookingType");
        }

        
        public ActionResult AdvanceBookRoom()
        {
            return View("BookRoom", new Tuple<Models.Rooms, Models.Calendar>(new Models.Rooms(), new Models.Calendar() { date = System.DateTime.Now}) );
        }

        public ActionResult ImmediateBookRoom()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QueryRoom(Models.Rooms room)
        {
            return Content("");
        }

        
        public ActionResult SpecificRoomSearch()
        {
            return PartialView("SpecificRoomQuery");
        }


        public ActionResult GeneralRoomSearch()
        {
            return PartialView("GeneralRoomQuery");
        }

        public ActionResult Book(Models.Rooms room)
        {
            return View();
        }
    }
}