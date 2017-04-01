using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;


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
            System.Diagnostics.Debug.WriteLine("Query Room" + room.id);
            return Content("");
        }

        [HttpPost]
        public ActionResult SelectRoom(Models.Rooms room)
        {
            Models.Calendar cal = new Models.Calendar();
            cal.room = room;
            cal.date = System.DateTime.Now;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM Bookings WHERE roomID='" + room.id + "'"; 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows) {
                        while (reader.Read())
                        {
                            Models.Bookings booking = new Models.Bookings();
                            booking.id = (int)reader[0];
                            booking.userID = (int)reader[1];
                            booking.startTime = (System.DateTime)reader[2];
                            booking.endTime = (System.DateTime)reader[3];
                            booking.roomID = (int)reader[4];

                            cal.bookings.Add(booking);
                        }
                    }
                    else
                    {
                        return View("SpecificRoomQueryError", new Models.Rooms());
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("SQL error");
                }
            }

            return View("RoomCalendar", cal);
        }

        public ActionResult SelectTime(Models.Calendar cal) {
            Models.Bookings booking = new Models.Bookings();

            booking.startTime = cal.date;
            booking.endTime = cal.date.AddHours(1.5);
            booking.roomID = cal.room.id;
            booking.id = -1;
            booking.userID = (int)Session["user"];

            return View("ConfirmBooking", booking);
        }

        public ActionResult SpecificRoomSearch()
        {
            return PartialView("SpecificRoomQuery");
        }


        public ActionResult GeneralRoomSearch()
        {
            return PartialView("GeneralRoomQuery");
        }

        [HttpPost]
        public ActionResult Book(Models.Bookings booking)
        {
            return View();
        }        
    }
}