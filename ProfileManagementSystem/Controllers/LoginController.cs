using System;
using System.Configuration;
using System.Data.SQLite;
using System.Web.Mvc;
using ProfileManagementSystem.Models;
using System.Web.SessionState;
using System.Data;

namespace ProfileManagementSystem.Controllers
{
    [SessionState(SessionStateBehavior.Default)]
    public class LoginController : Controller
    {
        private static string userFirstName;
        private static string lastName;
        private static string role;
        private static int id;

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult CheckValidUser(LoginModel model)
        {
            string result = "Fail";
            string username = model.Email;
            string password = model.Password;
            try
            {
                string encryptedPassword = string.Empty;
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    encryptedPassword = Utility.Utility.Encrypt(model.Password);
                    if (AuthenticateFromDB(username, encryptedPassword))
                    {

                        Session["fName"] = userFirstName;
                        Session["lName"] = lastName;
                        Session["role"] = role;
                        Session["id"] = id + "_thumb.jpeg";
                        HttpContext.Session["lName"] = lastName;                      
                        result = "Success";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Utility.Utility.StoreError("Login_Index", ex.Message);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AfterLogin()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }
        private static bool AuthenticateFromDB(string loginNAme, String Password)
        {
            string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
            //SQLiteConnection conn;
            SQLiteCommand cmd;
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectString))
                {
                    cmd = new SQLiteCommand();
                    cmd.CommandText = @"SELECT * from profileUser where  email='" + loginNAme.ToLower() + "'  and password ='" + Password + "' and role ='Administrator'  and isActive =1 ";
                    cmd.Connection = conn;
                    conn.Open();
                    DataSet userdetail = new DataSet();
                    SQLiteDataAdapter adp = new SQLiteDataAdapter(cmd);
                    adp.Fill(userdetail);
                   // i =Convert.ToInt32(  cmd.ExecuteScalar().ToString());
                    if (userdetail !=null)
                    {
                        userFirstName = userdetail.Tables[0].Rows[0]["firstName"].ToString();
                        lastName = userdetail.Tables[0].Rows[0]["lastName"].ToString();
                        id = int.Parse( userdetail.Tables[0].Rows[0]["id"].ToString());
                        role = userdetail.Tables[0].Rows[0]["role"].ToString();
                        conn.Close();
                        //userFirstName = "sandeep";
                        return true;
                    }
                    //while (r.Read())
                    //{
                    //    co
                    //    //userFirstName = r["firstname"].ToString();
                    //    //lastName = r["lastName"].ToString();
                    //    //role = r["role"].ToString();
                    //    // conn.Close();
                    //    return true;
                    //}
                }
            }
            catch (Exception ex)
            {
                Utility.Utility.StoreError("AuthenticateFromDB", ex.Message);
            }
            return false;

        }


    }
}