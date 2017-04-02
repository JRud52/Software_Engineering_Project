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
                            dash.rooms = new List<Models.Rooms>();

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
                                tempUser.hash = (string)reader[4];
                                dash.users.Add(tempUser);
                            }
                            reader.Close();

                            queryString = "SELECT * FROM Rooms";
                            command = new SqlCommand(queryString, connection);
                            reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                Models.Rooms tempRoom = new Models.Rooms();
                                tempRoom.id = (int)reader[0];
                                tempRoom.descriptor = (string)reader[1];
                                tempRoom.capacity = (int)reader[2];
                                dash.rooms.Add(tempRoom);
                            }
                            reader.Close();
                            
                            Session["calendar"] = cal;
                            Session["adminDash"] = dash;
                            return View("AdminDashboard", dash);
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

        public ActionResult UserEdit(Models.AdminDashModel dash)
        {
            return View(dash);
        }

        public ActionResult RoomAdd()
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
            user.hash = Convert.ToBase64String(hashbytes);


            string insertString = "INSERT INTO Users (email, name, privilage, hash) VALUES ('" + user.email + "', '" + user.name + "', " + user.privilage + ", '" + user.hash + "')"; // put SELECT commands here
            string selectString = "SELECT * FROM Users WHERE email = '" + user.email + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(insertString, connection);
                SqlCommand command2 = new SqlCommand(selectString, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    SqlDataReader reader = command2.ExecuteReader();
                    reader.Read();
                    user.id = (int)reader[0];
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
            ((Models.AdminDashModel)Session["adminDash"]).users.Add(user);
            return View("AdminDashboard", (Models.AdminDashModel)Session["adminDash"]);
        }


        [HttpPost]
        public ActionResult UserEditMethod(Models.AdminDashModel user)
        {
            
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            if (settings == null)
            {
                return Content("Something went wrong. Try reloading the page.");
            }

            string connectionString = settings.ConnectionString;

            

            string insertString = "UPDATE Users SET";

            if (user.editUserNew.email != user.editUser.email)
            {
                insertString += " email = '" + user.editUserNew.email + "'";
            }
            if (user.editUserNew.name != user.editUser.name)
            {
                insertString += " name = '" + user.editUserNew.name + "'";
            }
            if (user.editUserNew.privilage != user.editUser.privilage)
            {
                insertString += " privilage = " + user.editUserNew.privilage;
            }
            if (user.editUserNew.hash != null && user.editUserNew.hash != user.editUser.hash)
            {
                byte[] bytes = new UTF8Encoding().GetBytes(user.editUserNew.hash);
                byte[] hashbytes;
                using (var algorithm = new SHA512Managed())
                {
                    hashbytes = algorithm.ComputeHash(bytes);
                }
                user.editUserNew.hash = Convert.ToBase64String(hashbytes);
                insertString += " hash = '" + user.editUserNew.hash + "'";
            }
            insertString += " WHERE id = " + user.editUser.id;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(insertString, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
            Models.AdminDashModel tempModel = (Models.AdminDashModel)Session["adminDash"];

            int index = tempModel.users.FindIndex(i => i.id == user.editUser.id);
            user.editUserNew.id = user.editUser.id;
            tempModel.users[index] = user.editUserNew;
            Session["adminDash"] = tempModel;

            return View("AdminDashboard", (Models.AdminDashModel)Session["adminDash"]);
        }


        [HttpPost]
        public ActionResult RoomAddMethod(Models.Rooms room)
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["soft_db"];
            if (settings == null)
            {
                return Content("Something went wrong. Try reloading the page.");
            }

            string connectionString = settings.ConnectionString;

            string insertString = "INSERT INTO Rooms (id, descriptor, capacity) VALUES (" + room.id + ", '" + room.descriptor + "', " + room.capacity + ")"; // put SELECT commands here

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(insertString, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
            ((Models.AdminDashModel)Session["adminDash"]).rooms.Add(room);
            return View("AdminDashboard", (Models.AdminDashModel)Session["adminDash"]);
        }

    }
}
