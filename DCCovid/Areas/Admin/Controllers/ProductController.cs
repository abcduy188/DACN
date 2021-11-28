using DCCovid.Models;
using DCCovid.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace DCCovid.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        // GET: Admin/Product
        public ActionResult Index()
        {
            var dao = db.Products.OrderBy(d => d.Name).ToList();
            return View(dao);
        }
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories.ToList().Where(d=>d.Type == "PRODUCT"), "ID", "Name");
            return View();
        }
        public ActionResult Edit(long id)
        {
            Product products = db.Products.Find(id);
            ViewBag.CategoryID = new SelectList(db.Categories.Where(d => d.Type == "PRODUCT").ToList(), "ID", "Name", products.CategoryID);
            return View(products);
        }
        #region Create with single img

        [HttpPost]
        [ValidateInput(false)]//chap nhan mã html
        public ActionResult Create(Product products, HttpPostedFileBase fileUpload)
        {
            ViewBag.CategoryID = new SelectList(db.Categories.Where(d => d.Type == "PRODUCT").ToList(), "ID", "Name");
            if (fileUpload == null)
            {
                SetAlert("Vui lòng chọn ảnh", "warning");
                return View();
            }
            else
            if (ModelState.IsValid)
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
                products.CreateBy = session.Name;
                products.Image = fileName;
                products.CreateDate = DateTime.Now;
                db.Products.Add(products);
                db.SaveChanges();
                SetAlert("Thêm sản phẩm thành công", "success");
                return RedirectToAction("Index", "Product");
            }
            return View("Index");
        }
        #endregion

        #region Edit with single img

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Product entity, HttpPostedFileBase fileUpload)
        {
            ViewBag.CategoryID = new SelectList(db.Categories.Where(d => d.Type == "PRODUCT").ToList(), "ID", "Name", entity.CategoryID);
            var products = db.Products.Find(entity.ID);
            var session = (UserLogin)Session["DUY"];
            entity.ModifiedBy = session.Name;
            entity.ModifiedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (fileUpload == null)
                {
                    products.Image = products.Image;
                }
                else
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Assets/Thumbnail/"), fileName);
                    fileUpload.SaveAs(path);
                    products.Image = fileName;
                }
                products.Name = entity.Name;
                products.CategoryID = entity.CategoryID;
                products.SeoTitle = entity.SeoTitle;
                products.Description = entity.Description;
                products.Price = entity.Price;
                products.PromotionPrice = entity.PromotionPrice;
                products.Status = entity.Status;
                products.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                SetAlert("Sửa tin tức thành công", "success");
                return RedirectToAction("Index", "Product");
            }
            return View("Index");
        }

        #endregion
     
        
        public ActionResult Delete(long id)
        {
            Product entity = db.Products.Find(id);
            entity.Status = false;
            db.SaveChanges();
            SetAlert("Xóa sản phẩm thành công", "success");
            return RedirectToAction("Index");
        }
    }
}