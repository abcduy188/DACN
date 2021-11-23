using DCCovid.Common;
using DCCovid.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Controllers
{
    public class PostController : BaseController
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index()
        {
            List<Image> listimg = db.Images.Where(d => d.Type == "POST").OrderBy(d => d.ID).ToList();
            ViewBag.ListImg = listimg;
            Load();
            return View(db.PostCMTs.ToList().Where(d => d.IsDelete == false && d.PostID == null && d.Status == 1));
        }
        #region json 
        //public ActionResult GetData()
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    var results = db.PostCMTs.ToList();
        //    var img = db.Images.Where(d => d.Type == "POST").ToList();

        //    return Json(new { Data = results, TotalItems = results.Count, listimg = img, TotalImg = img.Count }, JsonRequestBehavior.AllowGet);
        //}
        #endregion
        // GET: Posts/Details/5
        public ActionResult Details(long id)
        {
            var sesspost = new PostSes();
            sesspost.PostID = id;
            Session.Add("Post", sesspost);

            PostCMT post = db.PostCMTs.Find(id);
            ViewBag.post = post;
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.Cmt = db.PostCMTs.Where(d => d.PostID == id && d.PostID != null).ToList();
            List<Image> listimg = db.Images.Where(d => d.Type == "CMT").OrderBy(d => d.ID).ToList();
            List<Image> listimgpost = db.Images.Where(d => d.Type == "POST" && d.TypeID == id).OrderBy(d => d.ID).ToList();
            ViewBag.ListImg = listimg;
            ViewBag.ListImgPost = listimgpost;
            return View();
        }

        // GET: Posts/Create
        public ActionResult Create(long? id)
        {
            ViewBag.CateID = new SelectList(db.CategoryUserPosts.ToList(), "ID", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Create(PostCMT post)
        {
            ViewBag.CateID = new SelectList(db.CategoryUserPosts.ToList(), "ID", "Name");
            var sess = Session["MEMBER"] as UserLogin;
            var sesspost = Session["Post"] as PostSes;
            if (ModelState.IsValid)
            {
                post.CreateBy = sess.Name;
                post.UserID = sess.UserID;
                post.LikeCount = 0;
                post.CreateDay = DateTime.Now;
                post.Status = 0;
                db.PostCMTs.Add(post);
                db.SaveChanges();
                HttpFileCollectionBase files = Request.Files; 

                for (int i = 0; i < files.Count; i++) 
                {
                    var fileName = "";
                    HttpPostedFileBase fileUpload = files[i];
                    fileName = Path.GetFileName(fileUpload.FileName);
                    if (fileName != "")
                    {

                        var path = Path.Combine(Server.MapPath("~/Assets/Thumbnails/"), fileName);
                        fileUpload.SaveAs(path);
                        InsertIMG(post.ID, fileName, post.PostID); 
                    }

                }
                SetAlert("Bài viết của bạn đang chở QTV duyệt", "warning");
                if (post.PostID != null)
                {
                    string url = "/Post/Details/" + sesspost.PostID;
                    return Redirect(url);
                }
                return RedirectToAction("Index");
            }

            return View(post);
        }

        public ActionResult Like(long id, string strURL)
        {

            var sess = Session["MEMBER"] as UserLogin;
            Session.Add("url", strURL);
            PostCMT post = db.PostCMTs.Find(id);
            User user = db.Users.Find(sess.UserID);

            post.UserID = sess.UserID;
            post.Users.Add(user);
            post.LikeCount = post.Users.Count();
            db.SaveChanges();

            if (Session["url"] != null)
            {
                return Redirect(Session["url"].ToString());
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        public ActionResult DisLike(long id, string strURL)
        {

            var sess = Session["MEMBER"] as UserLogin;
            Session.Add("url", strURL);
            PostCMT post = db.PostCMTs.Find(id);
            User user = db.Users.Find(sess.UserID);

            post.UserID = sess.UserID;
            post.Users.Remove(user);
            post.LikeCount = post.Users.Count();
            db.SaveChanges();
            if (Session["url"] != null)
            {
                return Redirect(Session["url"].ToString());
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        // POST: Posts/Delete/5
        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            PostCMT post = db.PostCMTs.Find(id);
            post.IsDelete = true;
            db.SaveChanges();
            SetAlert("Đã xóa bài viết", "success");
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public void InsertIMG(long id, string fileName, long? postID)
        {
            var img = new Image();
            img.FileName = fileName;
            if (postID == null)
            {
                img.Type = "POST";
            }
            else
            {
                img.Type = "CMT";
            }

            img.TypeID = id;
            db.Images.Add(img);
            db.SaveChanges();
        }
    }
}