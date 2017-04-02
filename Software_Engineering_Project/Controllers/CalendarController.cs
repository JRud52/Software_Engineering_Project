using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;

namespace Software_Engineering_Project.Controllers
{
    public class CalendarController : Controller
    {
        public PartialViewResult UpdateCalendar(System.DateTime currentCalendarDate, bool next)
        {
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
                }
                else
                {
                    currentCalendarDate = currentCalendarDate.AddMonths(-1);
                }
            }

            ((Models.Calendar)Session["calendar"]).date = currentCalendarDate; 

            return PartialView("Calendar", (Models.Calendar)Session["calendar"] );
        }


        public PartialViewResult UpdateRoomCalendar(System.DateTime currentCalendarDate, int roomID, bool next)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM bookings WHERE roomID='" + roomID + "'";

            Models.Calendar cal = new Models.Calendar();
            cal.roomID = roomID;
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
                }
                else
                {
                    currentCalendarDate = currentCalendarDate.AddMonths(-1);
                }
            }

            cal.date = currentCalendarDate;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                                              
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
                catch (Exception ex)
                {
                    
                }

                return PartialView("RoomCalendar", cal);
            }
        }

        public PartialViewResult UpdateUserCalendar(System.DateTime currentCalendarDate, int userID, bool next)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM bookings WHERE userID='" + userID + "'";

            Models.Calendar cal = new Models.Calendar();
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
                }
                else
                {
                    currentCalendarDate = currentCalendarDate.AddMonths(-1);
                }
            }

            cal.date = currentCalendarDate;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Models.Bookings booking = new Models.Bookings();
                        booking.id = (int)reader[0];
                        booking.userID = (int)reader[1];
                        booking.startTime = (System.DateTime)reader[2];
                        booking.endTime = (System.DateTime)reader[3];
                        booking.roomID = (int)reader[4];
                        booking.description = (string)reader[5];

                        cal.bookings.Add(booking);
                    }
                }
                catch (Exception ex)
                {

                }

                return PartialView("Calendar", cal);
            }
        }

        public PartialViewResult GetCalendar(System.DateTime currentCalendarDate)
        {                            
            return PartialView("Calendar", new Models.Calendar() { date = currentCalendarDate });
        }

        public PartialViewResult GetDaySchedule(System.DateTime date)
        {
            return PartialView("DaySchedule", new Models.Calendar() { date = date });
        }        

        public ActionResult GetRoomDaySchedule(System.DateTime date, List<Models.Bookings> bookings, int roomID)
        {
            Models.Calendar cal = new Models.Calendar();
            cal.date = date;
            cal.bookings = bookings;
            cal.roomID = roomID;

            return View("DaySchedule", cal);
        }
    }
}