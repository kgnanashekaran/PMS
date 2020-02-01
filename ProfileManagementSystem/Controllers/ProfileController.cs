﻿using ProfileManagementSystem.Models;
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
                //SQLiteConnection conn;
                SQLiteCommand cmd;

                // conn = new SQLiteConnection(connectString);
                using (SQLiteConnection conn = new SQLiteConnection(connectString))
                {
                    cmd = new SQLiteCommand();
                    //cmd.CommandText = @"SELECT * from profileUser where id >= 264";
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
              //  SQLiteConnection conn;
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
                            // conn = new SQLiteConnection(connectString);
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
                            //string picname = fileName + _ext;
                            if (profile == null || profile == "")
                            {
                                using (SQLiteConnection conn = new SQLiteConnection(connectString))
                                {
                                    cmd = new SQLiteCommand();
                                    cmd.CommandText = @"insert into profileUser (firstName,lastName,email,designation,empno,photo,password,role,isDisplay,isActive)  values('" + fname + "','" + lname + "','" + email.ToLower() + "','" + designation + "','" + empCode + "','" + fileName + "','" + encryptedPassword + "','" + role + "','" + flagDisplay + "','" + flag + "')";
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
                                //_imgname = picname;
                                var _savePath = Server.MapPath("/assets/images/profiles/") + _dbImgname + _ext;
                               // ViewBag.Msg = _savePath;
                                // var path = _comPath;
                                // Saving Image in Original Mode
                                pic.SaveAs(_savePath);
                                // resizing image
                                MemoryStream ms = new MemoryStream();
                                WebImage img = new WebImage(_savePath);
                                // Image image = Image.FromFile(_savePath);
                                // Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                                // thumb.Save(Path.ChangeExtension(_savePath, "thumb"));
                                if (img.Width > 200)
                                    img.Resize(200, 200);
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
                            cmd.CommandText = @" update profileUser set firstName='" + fname + "',lastName='" + lname + "', designation='" + designation + "',empno='" + empCode + "',photo='" + fileName + "',role='" + role + "',isDisplay='" + flagDisplay + "',isActive='" + flag + "' where email ='" + email.ToLower() + "'";
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
                        cmd.CommandText = @" update profileUser set firstName='" + fname + "',lastName='" + lname + "', designation='" + designation + "',empno='" + empCode + "',role='" + role + "',isDisplay='" + flagDisplay + "',isActive='" + flag + "' where email ='" + email.ToLower() + "'";
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


    }

}
