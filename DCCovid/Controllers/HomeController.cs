using DCCovid.Common;
using DCCovid.Models;
using PagedList;
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
        public ActionResult Search(string KeyWord, int? page, int pageSize = 3)
        {
            IQueryable<Product> product = db.Products;
            if (!string.IsNullOrEmpty(KeyWord))
            {
                product = db.Products.Where(d => d.Name.Contains(KeyWord) || d.Description.Contains(KeyWord) && d.IsDelete == false);
            }
            ViewBag.KeyWord = KeyWord;
            int pageNum = (page ?? 1);

            ViewBag.Product = product.OrderBy(d => d.Name).ToPagedList(pageNum, pageSize);
            IQueryable<User> user = db.Users;
            if (!string.IsNullOrEmpty(KeyWord))
            {
                user = db.Users.Where(d => d.Name.Contains(KeyWord) && d.Isdelete == false);
            }
            
            int pageNumUser = (page ?? 1);
            ViewBag.User = user.OrderBy(d => d.Name).ToList() ;

            IQueryable<PostCMT> post = db.PostCMTs;
            if (!string.IsNullOrEmpty(KeyWord))
            {
                post = db.PostCMTs.Where(d => d.Text.Contains(KeyWord) && d.IsDelete == false && d.PostID == null);
            }

            ViewBag.img = db.Images.ToList();
            ViewBag.Post = post.OrderByDescending(d => d.CreateDay).ToList();
            return View();

        }
    }
}