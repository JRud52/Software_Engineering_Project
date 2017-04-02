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
            return View("GeneralRoomQuery", new Models.Rooms());
        }


        [HttpPost]
        public ActionResult QueryRoom(Models.Rooms room)
        {
            System.Diagnostics.Debug.WriteLine("ROOM DESC + " + room.descriptor);

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM Rooms WHERE Capacity>='" + room.capacity + "' AND descriptor='" + room.descriptor + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                Models.RoomSearchModel searchResults = new Models.RoomSearchModel();

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                       
                        while (reader.Read())
                        {
                            Models.Rooms returnedRoom = new Models.Rooms();
                            returnedRoom.id = (int)reader[0];
                            returnedRoom.descriptor = (string)reader[1];
                            returnedRoom.capacity = (int)reader[2];

                            searchResults.rooms.Add(returnedRoom);
                        }

                        return View("RoomQueryResults", searchResults);
                    }
                    else
                    {
                        return View("GeneralRoomQueryError", new Models.Rooms());
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("error" + ex.Message);
                }

                return View();         
            }            
        }

        [HttpPost]
        public ActionResult SelectRoom(Models.Rooms room)
        {
            Models.Calendar cal = new Models.Calendar();
            cal.roomID = room.id;
            cal.date = System.DateTime.Now;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM Rooms WHERE ID='" + room.id + "'"; 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (!reader.HasRows) {
                        return View("SpecificRoomQueryError", new Models.Rooms());
                    }

                    reader.Close();
                    queryString = "SELECT * FROM Bookings WHERE roomID='" + room.id + "'";
                    command = new SqlCommand(queryString, connection);
                    command.ExecuteReader();

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
                   
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("SQL error");
                }

                return View("RoomCalendar", cal);
            }          
        }

        public ActionResult SelectRoom(int roomID)
        {
            Models.Rooms room = new Models.Rooms();
            Models.Calendar cal = new Models.Calendar();            
            cal.date = System.DateTime.Now;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM Rooms WHERE ID='" + roomID + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        room.id = roomID;
                        room.descriptor = (string)reader[1];
                        room.capacity = (int)reader[2];
                        cal.roomID = roomID;
                    }
                    else {
                        return View("SpecificRoomQueryError", new Models.Rooms());
                    }

                    reader.Close();
                    queryString = "SELECT * FROM Bookings WHERE roomID='" + roomID + "'";
                    command = new SqlCommand(queryString, connection);
                    command.ExecuteReader();

                    if (reader.HasRows)
                    {
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

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("SQL error");
                }

                return View("RoomCalendar", cal);
            }
        }

        [HttpPost]
        public ActionResult SelectTime(Models.Calendar cal) {
            Models.Bookings booking = new Models.Bookings();

            booking.startTime = cal.date;
            booking.endTime = cal.date.AddHours(1.5);
            booking.roomID = cal.roomID;
            booking.id = -1;
            booking.userID = (int)Session["user"];

            return View("ConfirmBooking", booking);
        }

        public ActionResult SpecificRoomSearch()
        {
            return PartialView("SpecificRoomQuery", new Models.Rooms());
        }


        public ActionResult GeneralRoomSearch()
        {            
            return PartialView("GeneralRoomQuery", new Models.Rooms());
        }

        [HttpPost]
        public ActionResult Book(Models.Bookings booking)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM Bookings WHERE " + 
                "roomID='" + booking.roomID + "' AND " +
                "startTime>=" + booking.startTime + "' AND " +
                "endTime<=" + booking.endTime + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                Models.RoomSearchModel searchResults = new Models.RoomSearchModel();
                SqlDataReader executor = command.ExecuteReader();


                try
                {
                    connection.Open();
                    
                    if (executor.HasRows)
                    {
                        return View("BookingConflict");                           
                    }                   
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("SQL error");
                }
                executor.Close();

                booking.userID = ((Models.Users)Session["user"]).id;
                string sqlCommand = "INSERT INTO Bookings VALUES(" +
                    "'" + booking.userID+ "'" +
                    "'" + booking.startTime + "'" +
                    "'" + booking.endTime + "'" +
                    "'" + booking.roomID + "'" +
                    "'" + booking.description + "'";

                command = new SqlCommand(sqlCommand, connection);
                command.ExecuteNonQuery();
                
                return View("UserDashboard", new Tuple<Models.Users, Models.Calendar>((Models.Users)Session["user"], (Models.Calendar)Session["calendar"]));
            }
        }


        public ActionResult BookConflict(Models.Calendar cal)
        {
            return View();
        }
    }
}