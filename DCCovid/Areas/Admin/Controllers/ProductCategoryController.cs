using DCCovid.Common;
using DCCovid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Areas.Admin.Controllers
{
    public class ProductCategoryController : BaseController
    {

        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index()
        {
            ViewBag.ListCate = db.Categories.OrderBy(d => d.ID).ToList();
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.ParentID = new SelectList(db.Categories.Where(d => d.ParentID == null).ToList(), "ID", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category productCategory)
        {
            ViewBag.ParentID = new SelectList(db.Categories.Where(d => d.ParentID == null).ToList(), "ID", "Name");
            if (ModelState.IsValid)
            {
                var session = (UserLogin)Session["DUY"];
                productCategory.CreateBy = session.Name;
                productCategory.CreateDate = DateTime.Now;
                db.Categories.Add(productCategory);
                db.SaveChanges();
                SetAlert("Thêm danh muc thanh cong", "success");
                return RedirectToAction("Index", "ProductCategory");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            Category model = db.Categories.Find(id);
            ViewBag.ParentID = new SelectList(db.Categories.Where(d => d.ParentID == null).ToList(), "ID", "Name", model.ParentID);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(Category entity)
        {
            Category productCategory = db.Categories.Find(entity.ID);

            ViewBag.ParentID = new SelectList(db.Categories.Where(d => d.ParentID == null).ToList(), "ID", "Name", entity.ParentID);
            if (ModelState.IsValid)
            {
                var session = (UserLogin)Session["DUY"];
                productCategory.ModifiedBy = session.Name;
                productCategory.Name = entity.Name;
                productCategory.SeoTitle = entity.SeoTitle;
                productCategory.ParentID = entity.ParentID;
                productCategory.Status = entity.Status;
                productCategory.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                SetAlert("Chỉnh sửa danh mục thành công", "success");
                return RedirectToAction("Index", "ProductCategory");
            }
            return View("Index");
        }
        // GET: Admin/ProductCategory/Delete/5
        [HttpGet]
        public ActionResult Delete(long id)
        {
            var user = db.Categories.Find(id);
            user.IsDelete = true;
            db.SaveChanges();
            SetAlert("Xoa thanh cong", "error");
            return RedirectToAction("Index");
        }
    }
}