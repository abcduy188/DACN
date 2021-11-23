using DCCovid.Common;
using DCCovid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Controllers
{
    public class HomeController : BaseController
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index()
        {
            Load();
            return View();
        }
        public ActionResult Noti()
        {
            var count = Session["CountSes"] as CountSes;
            int counttime;
            if (count!= null)
            {
                counttime = count.Count;
                return Json(new { data = counttime }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                counttime = 0;
                return Json(new { data = counttime }, JsonRequestBehavior.AllowGet);
            }
           
        }    
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            var listcate = db.Categories.ToList();
            return View(listcate);
        }
    }
}