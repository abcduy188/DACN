﻿using DCCovid.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Controllers
{
    public class ProductController : BaseController
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        // GET: Product
        [ChildActionOnly]
        public PartialViewResult ProductCategory()
        {
            List<Category> model = db.Categories.Where(d=>d.Type == "PRODUCT").ToList();
            return PartialView(model);

        }
        public ActionResult Search(string KeyWord, int? page, int pageSize = 3)
        {
            IQueryable<Product> product = db.Products;
            if (!string.IsNullOrEmpty(KeyWord))
            {
                product = db.Products.Where(d => d.Name.Contains(KeyWord) || d.Description.Contains(KeyWord));
            }
            ViewBag.KeyWord = KeyWord;
            int pageNum = (page ?? 1);
            return View(product.OrderBy(d => d.Name).ToPagedList(pageNum, pageSize));

        }
        [ChildActionOnly]
        public PartialViewResult CategoryList()
        {
            List<Category> model = db.Categories.Where(d => d.Type == "PRODUCT").ToList();
            return PartialView(model);

        }
        public ActionResult Category(long CateID, int page = 1, int pageSize = 2)
        {

            IEnumerable<Product> product = db.Products.Where(d => d.CategoryID == CateID).OrderBy(d => d.Name); // lấy danh sách sp theo category
            Category cate = db.Categories.Find(CateID); //lấy thông tin cate theo id
            var child = db.Categories.Where(d => d.ParentID == cate.ID && d.Type == "PRODUCT").OrderBy(d => d.Name).ToList(); //lấy ra list cate child theo cate
            if (child.Count() != 0)
            {
                foreach (var item in child)
                {
                    var list = db.Products.Where(d => d.CategoryID == item.ID).OrderBy(d => d.Name).ToList(); //lấy danh sách sp theo cate child
                    product = product.Concat(list); //nỗi vào danh sách sản phẩm đầu tiên
                }
            }

            var result = product.ToPagedList(page, pageSize); // chuyển danh sách sản phẩm theo page list || nếu dùng pagelist trực tiếp sẽ lỗi
            return View(result);
        }
        public ActionResult Detail(long id)
        {
            Product product = db.Products.Find(id);
            ViewBag.ListRele = db.Products.Where(d => d.CategoryID == product.CategoryID || d.Category.ParentID == product.Category.ParentID).ToList();
            return View(product);
        }
    }
}