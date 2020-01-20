using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SQLite;
using System.Data;

namespace ProfileManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        SQLiteConnection conn;
        SQLiteCommand cmd;
        SQLiteDataAdapter adapter;
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        String connectString;

        public HomeController()
        {

        }
        public ActionResult Index()
        {

            return View();
        }

    }
}