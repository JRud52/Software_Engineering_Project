using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Common;

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
        public ActionResult LogOn(Models.Users user)
        {
            System.Diagnostics.Debug.WriteLine("started connection");
            //ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_database"];
            //if (settings == null)
                //return View(user);
            //string connectionString = settings.ConnectionString;


            string connectionString = ConfigurationManager.ConnectionStrings["soft_db"].ConnectionString;
            /*
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.Add("Server", "tcp:soft-server.database.windows.net,1433");
            builder.Add("Initial Catalog", "soft_db");
            builder.Add("Persist Security Info", false);
            builder.Add("User ID", "soft_user");
            builder.Add("Password", "twoMoreThan3");
            builder.Add("MultipleActiveResultSets", false);
            builder.Add("Encrypt", true);
            builder.Add("TrustServerCertificate", false);
            builder.Add("Connection Timeout", 30);

            string connectionString = builder.ConnectionString;
            */

            string queryString = "SELECT * FROM Users"; // put SELECT commands here
            int paramValue = 5;
            System.Diagnostics.Debug.WriteLine("did the thing");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                //SqlCommand cmd = new SqlCommand();
                //cmd.CommandText = ""; // put INSERT commands here here
                //cmd.CommandType = CommandType.Text;

                //cmd.Parameters.AddWithValue("@id", 3);
                //cmd.Parameters.AddWithValue("@name", "testPerson");

                //cmd.Connection = connection;
                System.Diagnostics.Debug.WriteLine("made it this far");

                try
                {
                    connection.Open();
                    //cmd.ExecuteNonQuery();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("connected");
                        System.Diagnostics.Debug.WriteLine("\t{0}\t{1}\t{2}\t{3}\t{4}", reader[0], reader[1], reader[2], reader[3], reader[4]);
                    }
                    reader.Close();
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("not connected");
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }


                // System.Diagnostics.Debug.WriteLine("");
                /*
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

                */


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