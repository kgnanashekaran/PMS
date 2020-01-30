using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SQLite;
using System.Data;
using ProfileManagementSystem.Models;
using System.Configuration;
using System.Web.SessionState;

namespace ProfileManagementSystem.Controllers
{
    [SessionState(SessionStateBehavior.Default)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["fName"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var a = Utility.Utility.Decrypt("txnLZNQg1lzddjjocxNILw==");
                ViewData["UserProfiles"] = GetUserProfileList();
                ViewData["SOP"] = GetSOPList();


                //ViewData["UserProfiles"] = null;
                //ViewData["SOP"] = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        public ActionResult Index1()
        {
            try
            {
                ViewData["UserProfiles"] = GetUserProfileList();
                ViewData["SOP"] = GetSOPList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        public ActionResult Index2()
        {
            try
            {
                //ViewData["UserProfiles"] = GetUserProfileList1();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        public ActionResult Index3()
        {
            try
            {
                ViewData["UserProfiles"] = GetUserProfileList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
       

        private static List<profileUser> GetUserProfileList()
        {
            List<profileUser> _lstProfileUser = new List<profileUser>();

            string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
            SQLiteConnection conn;
            SQLiteCommand cmd;

            conn = new SQLiteConnection(connectString);
            cmd = new SQLiteCommand();
            cmd.CommandText = @"SELECT * from profileUser where isActive=1 and isDisplay=1";
            cmd.Connection = conn;
            conn.Open();

            SQLiteDataReader r = cmd.ExecuteReader();

            while (r.Read())
            {
                profileUser _profileUser = new profileUser();
                _profileUser.firstname = r["firstname"].ToString();
                _profileUser.lastname = r["lastname"].ToString();
                _profileUser.designation = r["designation"].ToString();
                _profileUser.empno = r["empno"].ToString();
                _profileUser.photo = r["photo"].ToString();
                _lstProfileUser.Add(_profileUser);
            }

            conn.Close();

            return _lstProfileUser;
        }

        private static List<SOP> GetSOPList()
        {
            List<SOP> _lstSOP = new List<SOP>();

            string connectString = ConfigurationManager.AppSettings["SQliteConnectionString"].ToString();
            SQLiteConnection conn;
            SQLiteCommand cmd;

            conn = new SQLiteConnection(connectString);
            cmd = new SQLiteCommand();
            cmd.CommandText = @"SELECT * from sop where isActive=1 order by sortorder";
            cmd.Connection = conn;
            conn.Open();

            SQLiteDataReader r = cmd.ExecuteReader();

            while (r.Read())
            {
                SOP _sop = new SOP();
                _sop.name = r["name"].ToString();
                _sop.filename = r["id"].ToString();
                _lstSOP.Add(_sop);
            }

            conn.Close();
          
            return _lstSOP;
        }

        private static List<profileUser> GetUserProfileList_old()
        {
            List<profileUser> _lstProfileUser = new List<profileUser>();
            using (SQLiteConnection connect = new SQLiteConnection(@"Data Source=D:\Shekar\Raju\PMS_Git\ProfileManagementSystem\App_Data\pmsdb.db;version=3"))
            //using (SQLiteConnection connect = new SQLiteConnection(@"ConfigurationManager.AppSettings["SQliteConnectionString"].ToString()")
            {
                connect.Open();
                using (SQLiteCommand fmd = connect.CreateCommand())
                {
                    fmd.CommandText = @"SELECT * from profileUser";
                    fmd.CommandType = CommandType.Text;
                    SQLiteDataReader r = fmd.ExecuteReader();

                    while (r.Read())
                    {
                        profileUser _profileUser = new profileUser();
                        _profileUser.firstname = r["name"].ToString();
                        _profileUser.designation = r["designation"].ToString();
                        _profileUser.empno = r["empno"].ToString();
                        _profileUser.photo = r["photo"].ToString();
                        _lstProfileUser.Add(_profileUser);
                    }

                    connect.Close();
                }
            }
            return _lstProfileUser;
        }
    }
}