using System;
using System.Configuration;
using System.Data.SQLite;
using System.Web.Mvc;
using ProfileManagementSystem.Models;
namespace ProfileManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        private static string userFirstName;
        private static string lastName;
        private static string role;

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            try
            {
                string encryptedPassword = string.Empty;
                if (!string.IsNullOrEmpty(model.UserName) && !string.IsNullOrEmpty(model.Password))
                {
                    encryptedPassword = Utility.Utility.Encrypt(model.Password);
                    if (AuthenticateFromDB(model.UserName, encryptedPassword))
                    {

                        //HttpContext.Session["fName"] = userFirstName;
                        //HttpContext.Session["lName"] = lastName;
                        //HttpContext.Session["role"] = role;
                       // ViewBag.NotValidUser = "Valid";
                       // return RedirectToAction("Index", "Profile");
                        return View("Index");
                    }
                    else
                    {
                        ViewBag.NotValidUser = "Invalid user";
                        return View("Index");
                        // return View("Index");
                    }
                }
                else
                {
                    ViewBag.NotValidUse = "User name or password should not be empty";
                    return View("Index");
                    //return Json(Convert.ToString("User Name or password should not be blank !"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Utility.Utility.StoreError("Login_Index", ex.Message);
                return Json(Convert.ToString("Request not processed"), JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public JsonResult Authenticate(string userName, String Password)
        //{
        //    //var userName = collection["userName"];
        //    ////var Password = collection["Password"];
        //    //return Json(Convert.ToString(""), JsonRequestBehavior.AllowGet);
        //    string encryptedPassword = string.Empty;
        //    if (userName != "" && Password != "")
        //    {
        //        encryptedPassword= Utility.Utility.Encrypt(Password);
        //        if (AuthenticateFromDB(userName, encryptedPassword))
        //        {
        //            //return RedirectToAction("Index", "Profile");
        //            //return Json(Convert.ToString("Successfull Login"), JsonRequestBehavior.AllowGet);
        //            return Json(new { Result = "Suc", Message = "hello" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(Convert.ToString("User Name or password should not be blank !"), JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(Convert.ToString("User Name or password is not correct !"), JsonRequestBehavior.AllowGet);
        //}
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
                    cmd.CommandText = @"SELECT * from profileUser where  email='" + loginNAme.ToLower() + "'  and password ='" + Password + "' and role ='Administrator' ";
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {

                        //userFirstName = r["firstname"].ToString();
                        //lastName = r["lastName"].ToString();
                        //role = r["role"].ToString();
                        // conn.Close();
                        return true;
                    }
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