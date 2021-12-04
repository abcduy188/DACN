using DCCovid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Controllers
{
    public class NewsController : Controller
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        // GET: News
        [ChildActionOnly]
        public PartialViewResult NewCategory()
        {
            var model = db.Categories.Where(d=>d.Type == "NEWS" && d.Status == true && d.IsDelete!= true).ToList();
            return PartialView(model);

        }
        public ActionResult Category(long id)
        {
            List<News> news = db.News.Where(d => d.CategoryID == id && d.IsDelete == false).ToList();
            return View(news);
        }
        public ActionResult DetailNew(long id)
        {
            ViewBag.img = db.Images.Where(d => d.Type == "NEWS" && d.TypeID == id).ToList();
            News news = db.News.Find(id);
            news.ViewCount += 1;
            db.SaveChanges();
            return View(news);
        }
    }
}