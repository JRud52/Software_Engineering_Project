﻿using System;
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
using System.Diagnostics;

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

                    byte[] bytes = new UTF8Encoding().GetBytes(user.hash);
                    byte[] hashbytes;
                    using (var algorithm = new SHA512Managed())
                    {
                        hashbytes = algorithm.ComputeHash(bytes);
                    }
                    string hash = Convert.ToBase64String(hashbytes);

                    Debug.WriteLine(hash);

                    reader.Read();
                    if (hash == reader[4].ToString())
                    {
                        user.email = reader[1].ToString();
                        user.name = reader[2].ToString();
                        user.privilage = (int)reader[3];

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

                            return PartialView("AdminDashboard", dash);
                        }
                        else
                            return PartialView("UserDashboard", user);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            // show the passed data in a seperate page.
            return PartialView("LoginPartial"); //Content("Username " + user.email + " <br/>Password: " + user.hash); //View(user);
        }

        public ActionResult DashboardUpdate()
        {
            return View();
        }
    }
}