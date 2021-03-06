﻿using System.Web;
using System.Web.Optimization;

namespace ProfileManagementSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/style").Include(
                    "~/assets/styles/main.css",
                    "~/assets/styles/jquery.simplyscroll.css"
                    /*"~/assets/font_awesome/css/font-awesome.min.css"*/));

            bundles.Add(new ScriptBundle("~/bundles/bottomScripts").Include(
                       "~/Scripts/main.js",
                       "~/Scripts/jquery-3.3.1.min.js",
                       "~/Scripts/jquery.easy-ticker.min.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/topScripts").Include(
                        "~/Scripts/jquery-3.3.1.min.js",
                        "~/Scripts/jquery.easing.min.js"
                        ));


        }
    }
}
