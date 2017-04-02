using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

namespace Software_Engineering_Project.Controllers
{
    public class UserController : Controller
    {
        private enum PRIVILEGES {STUDENT, FACULTY, ADMIN};

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Models.Users user)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            if (settings == null)
            {
                return Content("Something went wrong. Try reloading the page.");
            }

            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM Users WHERE email='" + user.email + "'"; // put SELECT commands here

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    byte[] bytes = new UTF8Encoding().GetBytes(user.hash);
                    byte[] hashbytes;
                    using (var algorithm = new SHA512Managed())
                    {
                        hashbytes = algorithm.ComputeHash(bytes);
                    }
                    string hash = Convert.ToBase64String(hashbytes);


                    reader.Read();
                    if (hash == reader[4].ToString())
                    {
                        user.id = (int)reader[0];
                        user.email = reader[1].ToString();
                        user.name = reader[2].ToString();
                        user.privilage = (int)reader[3];

                        Session["user"] = user;

                        Models.Calendar cal = new Models.Calendar() { date = System.DateTime.Now };


                        if (user.privilage == 3) {
                            reader.Close();

                            Models.AdminDashModel dash = new Models.AdminDashModel();
                            dash.id = user.id;
                            dash.name = user.name;
                            dash.email = user.email;
                            dash.privilage = user.privilage;
                            dash.users = new List<Models.Users>();

                            queryString = "SELECT * FROM Users";
                            command = new SqlCommand(queryString, connection);
                            reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                Models.Users tempUser = new Models.Users();
                                tempUser.id = (int)reader[0];
                                tempUser.email = (string)reader[1];
                                tempUser.name = (string)reader[2];
                                tempUser.privilage = (int)reader[3];
                                dash.users.Add(tempUser);
                            }
                            return View("AdminDashboard", new Tuple<Models.AdminDashModel, Models.Calendar>(dash, cal));
                        }
                        else
                        {
                            reader.Close();
                            queryString = "SELECT * FROM bookings WHERE userID='" + user.id + "'";
                            command = new SqlCommand(queryString, connection);
                            reader = command.ExecuteReader();


                            while (reader.Read()) {
                                Models.Bookings booking = new Models.Bookings();
                                booking.id = (int)reader[0];
                                booking.userID = (int)reader[1];
                                booking.startTime = (System.DateTime)reader[2];
                                booking.endTime = (System.DateTime)reader[3];
                                booking.roomID = (int)reader[4];
                                booking.description = (string)reader[5];

                                cal.bookings.Add(booking);
                            }

                            Session["calendar"] = cal;
                            return View("UserDashboard", new Tuple<Models.Users, Models.Calendar>(user, cal));
                        }

                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            
            return View("LoginError");
        }

        public ActionResult UserAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserAddMethod(Models.Users user)
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            if (settings == null)
            {
                return Content("Something went wrong. Try reloading the page.");
            }

            string connectionString = settings.ConnectionString;

            byte[] bytes = new UTF8Encoding().GetBytes(user.hash);
            byte[] hashbytes;
            using (var algorithm = new SHA512Managed())
            {
                hashbytes = algorithm.ComputeHash(bytes);
            }
            string hash = Convert.ToBase64String(hashbytes);


            string queryString = "INSERT INTO Users (email, name, privilage, hash) VALUES (" + user.email + ", " + user.name + ", " + user.privilage + ", " + hash + ")"; // put SELECT commands here

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    IAsyncResult result = command.BeginExecuteNonQuery();
                    command.ExecuteNonQuery();
                    command.EndExecuteNonQuery(result);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return View("AdminDashboard");
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home", null);
        }

        public ActionResult GetDashboard()
        {
            if (((Models.Users)Session["user"]).privilage == 3)
            {
                return View("AdminDashboard");
            }
            else
            {
                return View("UserDashboard", new Tuple<Models.Users, Models.Calendar>((Models.Users)Session["user"], (Models.Calendar)Session["calendar"]));
            }
        }
    }
}
