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
using System.Text;

namespace Software_Engineering_Project.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.Users user)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            if (settings == null)
                return Content("Something went wrong. Try reloading the page.");
            string connectionString = settings.ConnectionString;

            string queryString = "SELECT * FROM Users WHERE email='" + user.email + "'"; // put SELECT commands here

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    //string hash = UTF8Encoding.GetBytes(user.hash);

                    reader.Read();
                    if (reader[4].ToString() == user.hash)
                    {
                        user.email = reader[1].ToString();
                        user.name = reader[2].ToString();
                        user.privilage = (int)reader[3];

                        if (user.name == "admin")
                            return PartialView("AdminDashboard", user);
                        else
                            return PartialView("UserDashboard", user);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            // show the passed data in a seperate page.
            return PartialView("LoginPartial"); //Content("Username " + user.email + " <br/>Password: " + user.hash); //View(user);
        }
    }
}