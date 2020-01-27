﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Data.SQLite;
using ProfileManagementSystem.Models;

namespace ProfileManagementSystem.Controllers
{
    public class SOPController : Controller
    {
        string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
        SQLiteConnection conn;
        SQLiteCommand cmd;
        string user = string.Empty;
        // GET: SOP
        public ActionResult Index()
        {
            //try
            //{
            //    if (Session["fName"] == null || string.IsNullOrEmpty(user))
            //    {
            //        return RedirectToAction("Index", "Login");

            //    }
            //}
            //catch (Exception ex)
            //{
            //    Utility.Utility.StoreError("SOP_Index", ex.Message);
            //    return RedirectToAction("Index", "Login");

            //}
            return View();
        }

        [HttpPost]
        public ActionResult UpdateSOP(FormCollection collection)
        {
            try
            {

                //if (Session["fName"] == null)
                //{
                //    return RedirectToAction("Index", "Login");

                //}
                string _imgname = string.Empty;
                var userPicName = collection["Name"];
                var isActive = collection["IsActive"];
                var sortOrder = collection["SortOrder"];
                var Id = collection["Id"];
                int flag = 0;
                if (isActive == "Yes")
                {
                    flag = 1;
                }
                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic != null && pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    var _ext = Path.GetExtension(pic.FileName);
                    var uploadedfilename = fileName ;
                    if (_ext.ToLower() == ".jpg" || _ext.ToLower() == ".jpeg")
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(connectString))
                        {
                            cmd = new SQLiteCommand();
                            cmd.CommandText = @"update sop set filename='" + uploadedfilename.ToLower() + "' , sortorder='" + sortOrder + "' ,isActive='" + flag + "' where name = '" + userPicName.ToLower() + "' ";
                            cmd.Connection = conn;
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            //conn.Close();
                        }
                        string picname = "";
                        using (SQLiteConnection conn = new SQLiteConnection(connectString))
                        {
                            cmd = new SQLiteCommand();
                            cmd.CommandText = @" select max(id) from sop where name = '" + userPicName.ToLower() + "'";
                            cmd.Connection = conn;
                            conn.Open();
                            picname = cmd.ExecuteScalar().ToString();
                            conn.Close();
                        }

                        var _comPath = Server.MapPath("/assets/images/SOP/") + picname + _ext;
                        var path = _comPath;
                        pic.SaveAs(path);
                        return Json(Convert.ToString("SOP details has been updated"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(Convert.ToString("Formate not supported"), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    conn = new SQLiteConnection(connectString);
                    cmd = new SQLiteCommand();
                    //if (conn.State == System.Data.ConnectionState.Open)
                    //    conn.Close();
                   // SQLiteConnection.ClearAllPools();
                    //using (SQLiteConnection conn = new SQLiteConnection(connectString))
                    //{
                        cmd.CommandText = @"update sop set sortorder='" + sortOrder + "' ,isActive='" + flag + "' where name = '" + userPicName.ToLower() + "' ";
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                   // }
                    return Json(Convert.ToString("SOP details has been updated"), JsonRequestBehavior.AllowGet);
                }
                // return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                Utility.Utility.StoreError("SOP_UpdateSOP", ex.Message);
                return Json(Convert.ToString("Request not processed"), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult UploadFiles(FormCollection collection)
        {
            try
            {
                //if (Session["fName"] == null)
                //{
                //    return RedirectToAction("Index", "Login");

                //}
                string _imgname = string.Empty;
                //if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                //{
                var userPicName = collection["Name"];
                var isActive = collection["IsActive"];
                var sortOrder = collection["SortOrder"];
                int flag = 0;
                if (isActive == "Yes")
                {
                    flag = 1;
                }

                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    var _ext = Path.GetExtension(pic.FileName);
                    if (_ext.ToLower() == ".jpg" || _ext.ToLower() == ".jpeg")
                    {
                        string picname = "";
                        //conn = new SQLiteConnection(connectString);
                        using (SQLiteConnection conn = new SQLiteConnection(connectString))
                        {
                            cmd = new SQLiteCommand();
                            cmd.CommandText = @" select max(id) from sop where name = '" + pic.FileName.ToLower() + "'";
                            cmd.Connection = conn;
                            conn.Open();
                            picname = cmd.ExecuteScalar().ToString();
                           // conn.Close();
                        }
                        if (picname == null || picname == "")
                        {
                            using (SQLiteConnection conn = new SQLiteConnection(connectString))
                            {
                                cmd = new SQLiteCommand();
                                cmd.CommandText = @"insert into sop (name,filename,sortorder,isActive) VALUES('" + userPicName.ToLower() + "','" + userPicName.ToLower() + "' ,'" + sortOrder + "',  '" + flag + "')";
                                cmd.Connection = conn;
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                //conn.Close();
                            }
                            using (SQLiteConnection conn = new SQLiteConnection(connectString))
                            {
                                cmd = new SQLiteCommand();
                                cmd.CommandText = @"select max(id) from sop";
                                cmd.Connection = conn;
                                conn.Open();
                                _imgname = cmd.ExecuteScalar().ToString();
                                //conn.Close();
                            }
                            //_imgname = picname;
                            var _comPath = Server.MapPath("/assets/images/SOP/") + _imgname + _ext;
                            ViewBag.Msg = _comPath;
                            var path = _comPath;
                            // Saving Image in Original Mode
                            pic.SaveAs(path);
                            return Json(Convert.ToString("SOP Saved !"), JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(Convert.ToString("SOP already exist !"), JsonRequestBehavior.AllowGet);

                        }
                    }
                    else
                    {
                        return Json(Convert.ToString("Formate not supported"), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(Convert.ToString("Please choose file to upload !"), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Utility.Utility.StoreError("SOP_UploadFiles", ex.Message);
                return Json(Convert.ToString("Request not processed"), JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetSOPList()
        {
            try
            {

                //if (Session["fName"] == null)
                //{
                //    return RedirectToAction("Index", "Login");

                //}
                var result = GetSOPListFromDB();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Utility.Utility.StoreError("SOP_GetSopList", ex.Message);
                return Json(Convert.ToString("Request not processed"), JsonRequestBehavior.AllowGet);
            }
        }
        private static List<SOP> GetSOPListFromDB()
        {
            try
            {
                List<SOP> _lstSOP = new List<SOP>();
                string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
               // SQLiteConnection conn;
                SQLiteCommand cmd;
                SQLiteConnection.ClearAllPools();
                using (SQLiteConnection conn = new SQLiteConnection(connectString))
                {

                    //conn = new SQLiteConnection(conn);
                    cmd = new SQLiteCommand();
                    cmd.CommandText = @"SELECT * from sop order by sortorder";
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteDataReader r = cmd.ExecuteReader();

                    while (r.Read())
                    {
                        SOP _sop = new SOP();
                        _sop.name = r["name"].ToString();
                        _sop.filename = r["filename"].ToString();
                        _sop.status = r["isActive"].ToString();
                        _sop.sortOrder = r["sortorder"].ToString();
                        _sop.id = Convert.ToInt32(r["id"]);
                        _lstSOP.Add(_sop);
                    }
                }

               // conn.Close();

                return _lstSOP;
            }
            catch (Exception ex)
            {
                Utility.Utility.StoreError("SOP_GetSOPListFromDB", ex.Message);
                return null;
            }
        }
        //private static DataSet SaveSOP()
        //{
        //    List<profileUser> _lstProfileUser = new List<profileUser>();




        //    while (r.Read())
        //    {
        //        profileUser _profileUser = new profileUser();
        //        _profileUser.name = r["name"].ToString();
        //        _profileUser.designation = r["designation"].ToString();
        //        _profileUser.empno = r["empno"].ToString();
        //        _profileUser.photo = r["photo"].ToString();
        //        _lstProfileUser.Add(_profileUser);
        //    }

        //    conn.Close();

        //    return _lstProfileUser;
        //}
    }
}