using ProfileManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.SessionState;
namespace ProfileManagementSystem.Controllers
{
    [SessionState(SessionStateBehavior.Default)]
    public class ProfileController : Controller
    {
        string user = string.Empty;
        Alert alert;
        public ActionResult Index()
        {
            try
            {
                if (Session["fName"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                Utility.Utility.StoreError("Profile_Index", ex.Message);
            }
            return View();
        }
        public ActionResult GetProfileList()
        {
            if (HttpContext.Session["fName"] == null)
            {
                return RedirectToAction("Index", "Login");

            }
            var result = GetProfileListFromDB();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        private static List<profileUser> GetProfileListFromDB()
        {

            List<profileUser> _lstProfileUser = new List<profileUser>();

            try
            {

                string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
                SQLiteCommand cmd;

                using (SQLiteConnection conn = new SQLiteConnection(connectString))
                {
                    cmd = new SQLiteCommand();
                    cmd.CommandText = @"SELECT * from profileUser";
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteDataReader r = cmd.ExecuteReader();

                    while (r.Read())
                    {
                        profileUser _profileUser = new profileUser();
                        _profileUser.ID = r["id"].ToString();
                        _profileUser.firstname = r["firstname"].ToString();
                        _profileUser.email = r["email"].ToString();
                        _profileUser.designation = r["designation"].ToString();
                        _profileUser.empno = r["empno"].ToString();
                        _profileUser.photo = r["photo"].ToString();
                        _profileUser.role = r["role"].ToString();
                        _profileUser.status = Convert.ToInt32(r["isActive"].ToString());
                        _profileUser.display = Convert.ToInt32(r["isDisplay"].ToString());
                        _profileUser.lastname = r["lastName"].ToString(); 
                        _profileUser.sortOrder = r["sortorder"].ToString();
                        _lstProfileUser.Add(_profileUser);
                    }
                }
                //conn.Close();
            }
            catch (Exception ex)
            {
                Utility.Utility.StoreError("Profile_GetProfileListFromDB", ex.Message);
            }
            return _lstProfileUser;
        }

        [HttpPost]
        public ActionResult UploadProfiles(FormCollection collection, profileUser puser)
        {
            alert = new Alert();
            try
            {

                if (HttpContext.Session["fName"] == null)
                {
                    return RedirectToAction("Index", "Login");

                }
                string _dbImgname = string.Empty;
                string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
                SQLiteCommand cmd;
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var fname = collection["firstName"];
                    var lname = collection["lastName"];
                    var email = collection["email"];
                    var passoord = collection["password"];
                    var conPassword = collection["confirmPassword"];
                    var isActive = collection["IsActive"];
                    var role = collection["Role"];
                    var designation = collection["designation"];
                    var empCode = collection["empCode"];
                    var isDisplay = collection["isDisplay"];
                    var sortOrder = collection["SortOrder"];
                    string encryptedPassword = string.Empty;
                    int flag = 0;
                    if (isActive == "Yes")
                    {
                        flag = 1;
                    }
                    int flagDisplay = 0;
                    if (isDisplay == "Yes")
                    {
                        flagDisplay = 1;
                    }

                    if (!string.IsNullOrEmpty(passoord))
                    {
                        encryptedPassword = Utility.Utility.Encrypt(passoord);
                    }

                    var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                    if (pic.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(pic.FileName);
                        var _ext = Path.GetExtension(pic.FileName);
                        if (_ext.ToLower() == ".jpg" || _ext.ToLower() == ".jpeg")
                        {
                            string profile = "";
                            using (SQLiteConnection conn = new SQLiteConnection(connectString))
                            {
                                cmd = new SQLiteCommand();
                                cmd.CommandText = @" select max(id) from profileUser where email = '" + email.ToLower() + "'";
                                cmd.Connection = conn;
                                conn.Open();
                                profile = cmd.ExecuteScalar().ToString();
                                conn.Close();
                            }
                            if (profile == null || profile == "")
                            {
                                using (SQLiteConnection conn = new SQLiteConnection(connectString))
                                {
                                    cmd = new SQLiteCommand();
                                    cmd.CommandText = @"insert into profileUser (firstName,lastName,email,designation,empno,photo,password,role,isDisplay,isActive,sortorder)  values('" + fname + "','" + lname + "','" + email.ToLower() + "','" + designation + "','" + empCode + "','" + fileName + "','" + encryptedPassword + "','" + role + "','" + flagDisplay + "','" + flag + "','"+ sortOrder + "')";
                                    cmd.Connection = conn;
                                    conn.Open();
                                    cmd.ExecuteNonQuery();
                                }
                                using (SQLiteConnection conn = new SQLiteConnection(connectString))
                                {
                                    cmd = new SQLiteCommand();
                                    cmd.CommandText = @"select max(id) from profileUser";
                                    cmd.Connection = conn;
                                    conn.Open();
                                    _dbImgname = cmd.ExecuteScalar().ToString();
                                }                             
                                var _savePath = Server.MapPath("/assets/images/profiles/") + _dbImgname + _ext;
                               
                                // Saving Image in Original Mode
                                pic.SaveAs(_savePath);
                                // resizing image
                                MemoryStream ms = new MemoryStream();
                                WebImage img = new WebImage(_savePath);
                               
                                if (img.Width > 64)
                                    img.Resize(64, 64,true);
                                img.Save(Server.MapPath("/assets/images/profiles/") + _dbImgname + "_thumb" + ".jpg");
                                // end resize
                                alert.message = "Profile Saved !";
                                alert.status = true;
                                return Json(alert, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                alert.message = "Record already exist !";
                                alert.status = false;
                                return Json(alert, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            alert.message = "Formate not supported !";
                            alert.status = false;
                            return Json(alert, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        alert.message = "Please choose the picture to upload !";
                        alert.status = false;
                        return Json(alert, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    alert.message = "Please choose the picture to upload  !";
                    alert.status = false;
                    return Json(alert, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                alert.message = "Request not processed !";
                alert.status = false;
                Utility.Utility.StoreError("Profile_UploadProfiles", ex.Message);
                return Json(alert, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult UpdateProfile(FormCollection collection, profileUser puser)
        {
            alert = new Alert();
            try
            {

                if (HttpContext.Session["fName"] == null)
                {
                    return RedirectToAction("Index", "Login");

                }
                string _imgname = string.Empty;
                string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
               // SQLiteConnection conn;
                SQLiteCommand cmd;

                var fname = collection["firstName"];
                var lname = collection["lastName"];
                var email = collection["email"];
                var passoord = collection["password"];
                var conPassword = collection["confirmPassword"];
                var isActive = collection["IsActive"];
                var role = collection["Role"];
                var designation = collection["designation"];
                var empCode = collection["empCode"];
                var isDisplay = collection["isDisplay"];
                var sortOrder = collection["SortOrder"];
                int flag = 0;
                if (isActive == "Yes")
                {
                    flag = 1;
                }
                int flagDisplay = 0;
                if (isDisplay == "Yes")
                {
                    flagDisplay = 1;
                }

                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic != null && pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    var _ext = Path.GetExtension(pic.FileName);
                    var uploadedfilename = fileName + _ext;
                    if (_ext.ToLower() == ".jpg" || _ext.ToLower() == ".jpeg")
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(connectString))
                        {
                            cmd = new SQLiteCommand();
                            cmd.CommandText = @" update profileUser set firstName='" + fname + "',lastName='" + lname + "', designation='" + designation + "',empno='" + empCode + "',photo='" + fileName + "',role='" + role + "',isDisplay='" + flagDisplay + "',isActive='" + flag + "' ,sortorder='" + sortOrder + "' where email ='" + email.ToLower() + "'";
                            cmd.Connection = conn;
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            //conn.Close();
                        }
                        string picname = "";
                        using (SQLiteConnection conn = new SQLiteConnection(connectString))
                        {
                            cmd = new SQLiteCommand();
                            cmd.CommandText = @" select max(id) from profileUser where email = '" + email.ToLower() + "'";
                            cmd.Connection = conn;
                            conn.Open();
                            picname = cmd.ExecuteScalar().ToString();
                            //conn.Close();
                        }

                        var _comPath = Server.MapPath("/assets/images/profiles/") + picname + _ext;
                        var path = _comPath;
                        pic.SaveAs(path);

                        // resizing image
                        MemoryStream ms = new MemoryStream();
                        WebImage img = new WebImage(path);
                        
                        if (img.Width > 200)
                            img.Resize(200, 200);
                        img.Save(Server.MapPath("/assets/images/profiles/") + picname + "_thumb" + ".jpg");
                        // end resize
                        alert.message = "Profile updated !";
                        alert.status = true;
                        return Json(alert, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        alert.message = "Formate not supported !";
                        alert.status = false;
                        return Json(alert, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    using (SQLiteConnection conn = new SQLiteConnection(connectString))
                    {
                        // conn = new SQLiteConnection(connectString);
                        cmd = new SQLiteCommand();
                        cmd.CommandText = @" update profileUser set firstName='" + fname + "',lastName='" + lname + "', designation='" + designation + "',empno='" + empCode + "',role='" + role + "',isDisplay='" + flagDisplay + "',isActive='" + flag + "' ,sortorder='" + sortOrder + "' where email ='" + email.ToLower() + "'";
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        //conn.Close();

                        alert.message = "Profile updated  !";
                        alert.status = true;
                        return Json(alert, JsonRequestBehavior.AllowGet);
                    }
                }
                //return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                alert.message = "Request not processed !";
                alert.status = false;
                Utility.Utility.StoreError("Profile_UpdateProfiles", ex.Message);
                return Json(alert, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CheckDuplicateProfile(FormCollection collection)
        {
            alert = new Alert();
            string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
            var resultCount = "";
            SQLiteCommand cmd;
            string email = collection["email"];
            using (SQLiteConnection conn = new SQLiteConnection(connectString))
            {
                cmd = new SQLiteCommand();
                cmd.CommandText = @"select max(id) from profileUser where email ='" + email + "'";
                cmd.Connection = conn;
                conn.Open();
                resultCount = cmd.ExecuteScalar().ToString();
            }
            if (resultCount == null || resultCount == "")
            {
                alert.message = "0";
            }
            else
            {
                alert.message = "1";
            }
            alert.status = true;
            return Json(alert, JsonRequestBehavior.AllowGet);
        }

    }

}
