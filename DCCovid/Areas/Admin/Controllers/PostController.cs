using DCCovid.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Areas.Admin.Controllers
{
    public class PostController : BaseController
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        // GET: Admin/Post
        public ActionResult Index(int? page, int pageSize = 3)
        {
            IQueryable<PostCMT> list = db.PostCMTs.OrderByDescending(d=>d.CreateDay).Where(d => d.PostID == null);
            ViewBag.img = db.Images.Where(d => d.Type == "POST").ToList();
            int pageNum = (page ?? 1);
            return View(list.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Detail(long id)
        {
            PostCMT user = db.PostCMTs.Find(id);
            return PartialView(user);
        }
    
    public ActionResult Delete(long id)
        {
            var post = db.PostCMTs.Find(id);
            var cmt = db.PostCMTs.Where(d => d.PostID == post.ID).ToList();
            if(post.IsDelete == true)
            {
                post.IsDelete = false;
                SetAlert("Xoa thanh cong", "error");
            }
            else
            {
                foreach (var i in cmt)
                {
                    i.IsDelete = true;
                }
                post.IsDelete = true;
                SetAlert("khôi phục thành cong", "success");
            }
            
            db.SaveChanges();
           
            return RedirectToAction("Index");
        }
        public ActionResult Change(long id, string key)
        {
            var post = db.PostCMTs.Find(id);
            if(key == "accept")
            {
                post.Status = 1;
                
            }else
            {
                post.Status = -1;
            }
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}