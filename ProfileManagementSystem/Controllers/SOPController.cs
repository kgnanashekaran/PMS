using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Helpers;
using System.Data;
using System.Configuration;
using System.Data.SQLite;

namespace ProfileManagementSystem.Controllers
{
    public class SOPController : Controller
    {
        string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
        SQLiteConnection conn;
        SQLiteCommand cmd;  
        // GET: SOP
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadFiles(FormCollection collection)
        {
            string _imgname = string.Empty;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var userPicName = collection["Name"];
                var isActive = collection["IsActive"];
                int flag = 0;
                if(isActive=="Yes")
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
                        conn = new SQLiteConnection(connectString);
                        cmd = new SQLiteCommand();
                        cmd.CommandText = @" select max(id) from sop where name = '" + pic.FileName.ToLower()+"'";
                        cmd.Connection = conn;
                        conn.Open();
                        string picname = cmd.ExecuteScalar().ToString();
                        conn.Close();
                        if (picname == null || picname == "")
                        {
                            conn = new SQLiteConnection(connectString);
                            cmd = new SQLiteCommand();
                            cmd.CommandText = @"insert into sop (name,filename,sortorder,isActive) VALUES('" + userPicName.ToLower() + "','" + userPicName.ToLower() + "',1111,'" + flag + "')";
                            cmd.Connection = conn;
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();

                            cmd = new SQLiteCommand();
                            cmd.CommandText = @"select max(id) from sop";
                            cmd.Connection = conn;
                            conn.Open();
                            _imgname = cmd.ExecuteScalar().ToString();
                            conn.Close();

                            //_imgname = picname;
                            var _comPath = Server.MapPath("/assets/images/") + _imgname + _ext;
                            _imgname = "abc_1";
                            ViewBag.Msg = _comPath;
                            var path = _comPath;
                            // Saving Image in Original Mode
                            pic.SaveAs(path);
                            // resizing image
                            MemoryStream ms = new MemoryStream();
                            WebImage img = new WebImage(_comPath);
                            //Image image = Image.FromFile(fileName);
                            //Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                            //thumb.Save(Path.ChangeExtension(fileName, "thumb"));
                            if (img.Width > 200)
                                img.Resize(200, 200);
                            img.Save(_comPath);
                            // end resize
                        }
                        else
                        {
                            //record already exist
                        }
                    }
                    else
                    {
                        return Json(Convert.ToString("Formate not supported"), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);

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