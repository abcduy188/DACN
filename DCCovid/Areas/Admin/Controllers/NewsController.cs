using DCCovid.Common;
using DCCovid.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Areas.Admin.Controllers
{
    public class NewsController : BaseController
    {
        // GET: Admin/News
        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index()
        {
            ViewBag.ListNews = db.News.ToList();
            ViewBag.Catenews = new SelectList(db.Categories.Where(d => d.Type == "NEWS").ToList(), "ID", "Name");
            return View(db.News.OrderByDescending(d => d.CreateDate).ToList());
        }

        // GET: Admin/New/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories.Where(d=>d.Type=="NEWS").ToList(), "ID", "Name");
            return View();
        }

        #region Create with single img

        [HttpPost]
        [ValidateInput(false)]//chap nhan mã html
        public ActionResult Create(News @new, HttpPostedFileBase fileUpload)
        {
            ViewBag.CategoryID = new SelectList(db.Categories.Where(d => d.Type == "NEWS").ToList(), "ID", "Name");
            if (ModelState.IsValid)
            {
                if (fileUpload == null)
                {
                    SetAlert("Vui lòng chọn ảnh", "warning");
                    return View();
                }
                else
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Assets/Thumbnails/"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.thongbao = "Hình ảnh đã tồn tại";

                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }

                    var session = (UserLogin)Session["DUY"];
                    @new.CreateBy = session.Email;
                    @new.Image = fileName;
                    @new.CreateDate = DateTime.Now;
                    db.News.Add(@new);
                    db.SaveChanges();
                    SetAlert("Thêm tin tức thành công", "success");
                    return RedirectToAction("Index", "News");
                }
            }
            return View("Index");

            #endregion
        }
        // GET: Admin/New/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News @new = db.News.Find(id);
            if (@new == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories.Where(d => d.Type == "NEWS").ToList(), "ID", "Name", @new.CategoryID);
            return View(@new);
        }
        #region Edit with single img

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(News @new, HttpPostedFileBase fileUpload)
        {
            ViewBag.CategoryID = new SelectList(db.Categories.Where(d => d.Type == "NEWS").ToList(), "ID", "Name", @new.CategoryID);
            News news = db.News.Find(@new.ID);
            if (ModelState.IsValid)
            {
                if (fileUpload == null)
                {
                    var session = (UserLogin)Session["DUY"];
                    @new.ModifiedBy = session.Name;
                    @new.ModifiedDate = DateTime.Now;
                    news.Name = @new.Name;
                    news.CategoryID = @new.CategoryID;
                    news.SeoTitle = @new.SeoTitle;
                    news.Description = @new.Description;
                    try
                    {
                        if (!string.IsNullOrEmpty(news.Image))
                        {
                            news.Image = news.Image;
                        }
                        else
                        {
                            SetAlert("Vui lòng chọn ảnh sản phẩm", "warning");
                            return RedirectToAction("Edit", "New");
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Cap nhat khong thanh cong!!");
                    }
                    news.Detail = @new.Detail;
                    news.Status = @new.Status;
                    news.ModifiedDate = DateTime.Now;
                    news.ModifiedBy = session.Email;
                    db.SaveChanges();
                    SetAlert("Sửa tin tức thành công", "success");
                    return RedirectToAction("Index", "New");
                }
                else
                {

                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Assets/Thumbnails/"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    var session = (UserLogin)Session["DUY"];
                    news.Image = fileName;
                    news.Name = @new.Name;
                    news.CategoryID = @new.CategoryID;
                    news.SeoTitle = @new.SeoTitle;
                    news.Description = @new.Description;
                    news.Detail = @new.Detail;
                    news.Status = @new.Status;
                    news.ModifiedDate = DateTime.Now;
                    news.ModifiedBy = session.Email;
                    db.SaveChanges();
                    SetAlert("Sửa tin tức thành công", "success");
                    return RedirectToAction("Index", "New");
                }
            }
            return View("Index");
        }

        #endregion

        // GET: Admin/New/Delete/5
        [HttpGet]
        public ActionResult Delete(long id)
        {
            var user = db.News.Find(id);
            db.News.Remove(user);
            db.SaveChanges();
            SetAlert("Xoa thanh cong", "error");
            return RedirectToAction("Index");
        }
    }
}