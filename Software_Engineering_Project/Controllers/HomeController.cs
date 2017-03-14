using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Common;
using System.Security.Cryptography;

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
        public ActionResult Login(Models.Users user)
        {
            System.Diagnostics.Debug.WriteLine("started connection");
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            if (settings == null)
                return Content("Something went wrong. Try reloading the page.");
            string connectionString = settings.ConnectionString;

            //string connectionString = ConfigurationManager.ConnectionStrings["soft_db"].ConnectionString;

            string queryString = "SELECT hash FROM Users WHERE email='" + user.email + "'"; // put SELECT commands here

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
                    reader.Read();
                    if (reader[0].ToString() == user.hash)
                    {
                        return PartialView("Dashboard", user);
                    }

                    //while (reader.Read())
                    //{
                        //System.Diagnostics.Debug.WriteLine("connected");
                        //System.Diagnostics.Debug.WriteLine("\t{0}\t{1}\t{2}\t{3}\t{4}", reader[0], reader[1], reader[2], reader[3], reader[4]);
                    //}
                    reader.Close();
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("not connected");
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            // show the passed data in a seperate page.
            return PartialView("LoginPartial"); //Content("Username " + user.email + " <br/>Password: " + user.hash); //View(user);
        }

        [HttpPost]
        public ActionResult QueryRoom(Models.Rooms room)
        {            
            return Content(""); 
        }
    }
}