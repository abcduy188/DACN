using DCCovid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Areas.Admin.Controllers
{
    public class PostController : Controller
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        // GET: Admin/Post
        public ActionResult Index()
        {
            var list = db.PostCMTs.Where(d => d.PostID == null).ToList();

            return View(list);
        }
        public ActionResult Delete(long id)
        {
            var post = db.PostCMTs.Find(id);
            var cmt = db.PostCMTs.Where(d => d.PostID == post.ID).ToList();

            foreach(var i in cmt)
            {
                i.IsDelete = true;
            }
            post.IsDelete = true;
            db.SaveChanges();

            return View();
        }
    }
}