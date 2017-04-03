using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

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


        public ActionResult ReleaseRoom(int bookingID) {
           if (Session["user"] == null){
				Session.Clear();
            	return RedirectToAction("Index", "Home", null);
			}
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string commandString = "DELETE FROM bookings WHERE id=@bookingID";
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(commandString, connection);
                command.Parameters.Add("@bookingID", SqlDbType.Int);
                command.Parameters["@bookingID"].Value = bookingID;

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();                   
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("error" + ex.Message);
                }

                foreach (Models.Bookings booking in ((Models.Calendar)Session["calendar"]).bookings)
                {
                    if (booking.id == bookingID)
                    {
                        ((Models.Calendar)Session["calendar"]).bookings.Remove(booking);
                        break;
                    }
                }

                return View("UserDashboard", new Tuple<Models.Users, Models.Calendar>((Models.Users)Session["user"], (Models.Calendar)Session["calendar"]));
            }

        }

        public ActionResult ExtendBooking(int bookingID, int roomID, System.DateTime endTime, string description)
        {
			if (Session["user"] == null){
				Session.Clear();
            	return RedirectToAction("Index", "Home", null);
			}
				

            if (endTime.Hour > 20) {
                ViewData["error-message"] = "The last booking of the day is 8:30pm";
                return View("ExtendBookingError", new Tuple<Models.Users, Models.Calendar>((Models.Users)Session["user"], (Models.Calendar)Session["calendar"]));
            }

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM bookings WHERE roomID>=@roomID AND startTime=@endTime";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@roomID", SqlDbType.Int);
                command.Parameters["@roomID"].Value = roomID;

                command.Parameters.Add("@endTime", SqlDbType.DateTime);
                command.Parameters["@endTime"].Value = endTime;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        ViewData["error-message"] = "Someone has already reserved the room during that time.";
                        return View("ExtendBookingError", new Tuple<Models.Users, Models.Calendar>((Models.Users)Session["user"], (Models.Calendar)Session["calendar"]));
                    }
                    else
                    {
                        reader.Close();

                        Models.Bookings booking = new Models.Bookings();
                        booking.startTime = endTime;
                        booking.endTime = endTime.AddHours(1.5);
                        booking.userID = ((Models.Users)Session["user"]).id;
                        booking.roomID = roomID;
                        booking.description = "EXTENSION: Original Description - " + description;

                        string sqlCommand = "INSERT INTO Bookings VALUES(" +
                            "@userID, " +
                            "@startTime, " +
                            "@endTime, " +
                            "@roomID, " +
                            "@desc)";

                        command = new SqlCommand(sqlCommand, connection);
                        command.Parameters.Add("@userID", SqlDbType.Int);
                        command.Parameters["@userID"].Value = booking.userID;

                        command.Parameters.Add("@startTime", SqlDbType.DateTime);
                        command.Parameters["@startTime"].Value = booking.startTime;


                        command.Parameters.Add("@endTime", SqlDbType.DateTime);
                        command.Parameters["@endTime"].Value = booking.endTime;

                        command.Parameters.Add("@desc", SqlDbType.VarChar);
                        command.Parameters["@desc"].Value = booking.description;

                        command.Parameters.Add("@roomID", SqlDbType.Int);
                        command.Parameters["@roomID"].Value = booking.roomID;

                        command.ExecuteNonQuery();

                        queryString = "UPDATE bookings SET endTime='" + endTime.AddHours(1.5) + "' WHERE id='" + bookingID + "'";
                        command = new SqlCommand(queryString, connection);
                        command.ExecuteNonQuery();

                        ((Models.Calendar)Session["calendar"]).bookings.Add(booking);

                        return View("UserDashboard", new Tuple<Models.Users, Models.Calendar>((Models.Users)Session["user"], (Models.Calendar)Session["calendar"]));
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
        public ActionResult QueryRoom(Models.Rooms room)
        {
            System.Diagnostics.Debug.WriteLine("ROOM DESC + " + room.descriptor);

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM Rooms WHERE Capacity>=@capcity AND UPPER(descriptor)=@descriptor";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@capcity", SqlDbType.Int);
                command.Parameters["@capcity"].Value = room.capacity;

                command.Parameters.Add("@descriptor", SqlDbType.VarChar);
                command.Parameters["@descriptor"].Value = room.descriptor.ToUpper();


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
            cal.date = System.DateTime.Now;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM Rooms WHERE ID=@roomID";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@roomID", SqlDbType.Int);
                command.Parameters["@roomID"].Value = room.id;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        room.id = room.id;
                        room.descriptor = (string)reader[1];
                        room.capacity = (int)reader[2];
                        cal.roomID = room.id;
                    }
                    else
                    {
                        return View("SpecificRoomQueryError", new Models.Rooms());
                    }

                    reader.Close();
                    queryString = "SELECT * FROM bookings WHERE roomID=@roomID";
                    command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@roomID", SqlDbType.Int);
                    command.Parameters["@roomID"].Value = room.id;
                    reader = command.ExecuteReader();

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

                Session["bookings"] = cal.bookings;
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

            string queryString = "SELECT * FROM Rooms WHERE ID=@roomID";
            

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@roomID", SqlDbType.Int);
                command.Parameters["@roomID"].Value = roomID;

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
                    queryString = "SELECT * FROM bookings WHERE roomID=@roomID";
                    command = new SqlCommand(queryString, connection);
                    command.Parameters.Add("@roomID", SqlDbType.Int);
                    command.Parameters["@roomID"].Value = roomID;
                    reader = command.ExecuteReader();

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

                Session["bookings"] = cal.bookings;
                return View("RoomCalendar", cal);
            }
        }

        public ActionResult SelectTime(System.DateTime date, List<Models.Bookings> bookings, int roomID)
        {

            Models.Bookings booking = new Models.Bookings();

            booking.startTime = date;
            booking.endTime = date.AddHours(1.5);
            booking.roomID = roomID;
            booking.id = -1;
            booking.userID = ((Models.Users)Session["user"]).id;

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
			if (Session["user"] == null){
				Session.Clear();
            	return RedirectToAction("Index", "Home", null);
			}
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM bookings WHERE " + 
                "roomID=@roomID AND " +
                "startTime>=@startTime AND " +
                "endTime<=@endTime";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@roomID", SqlDbType.Int);
                command.Parameters["@roomID"].Value = booking.roomID;

                command.Parameters.Add("@startTime", SqlDbType.DateTime);
                command.Parameters["@startTime"].Value = booking.startTime;

                command.Parameters.Add("@endTime", SqlDbType.DateTime);
                command.Parameters["@endTime"].Value = booking.endTime;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();       

                    if (reader.HasRows)
                    {
                        return View("BookingConflict");                           
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("SQL error");
                }
                

                booking.userID = ((Models.Users)Session["user"]).id;
                string sqlCommand = "INSERT INTO Bookings VALUES(" +
                    "@userID, " +
                    "@startTime, " +
                    "@endTime, " +
                    "@roomID, " +
                    "@desc)";

                command = new SqlCommand(sqlCommand, connection);
                command.Parameters.Add("@userID", SqlDbType.Int);
                command.Parameters["@userID"].Value = booking.userID;

                command.Parameters.Add("@startTime", SqlDbType.DateTime);
                command.Parameters["@startTime"].Value = booking.startTime;


                command.Parameters.Add("@endTime", SqlDbType.DateTime);
                command.Parameters["@endTime"].Value = booking.endTime;

                command.Parameters.Add("@desc", SqlDbType.VarChar);
                command.Parameters["@desc"].Value = booking.description;

                command.Parameters.Add("@roomID", SqlDbType.Int);
                command.Parameters["@roomID"].Value = booking.roomID;

                command.ExecuteNonQuery();

                ((Models.Calendar)Session["calendar"]).bookings.Add(booking);

                return View("UserDashboard", new Tuple<Models.Users, Models.Calendar>((Models.Users)Session["user"], (Models.Calendar)Session["calendar"]));
            }
        }

        [HttpPost]
        public ActionResult BookConflict(Models.Calendar cal)
        {
            return View("Userdashboard", new Tuple<Models.Users, Models.Calendar>((Models.Users)Session["user"], cal));
        }
    }
}