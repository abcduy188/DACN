using DCCovid.Common;
using DCCovid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        // GET: Admin/User
        public ActionResult Index()
        {
            //var user = db.Users.OrderBy(d => d.Name).ToList();
            ViewBag.ListUser = db.Users.ToList();
            ViewBag.GroupID = new SelectList(db.User_Group.ToList(), "ID", "Name");
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.User_Group.ToList(), "ID", "Name");
            return View();
        }

       
        public ActionResult Edit(long id )
        {
            var user = db.Users.Find(id);
            ViewBag.GroupID = new SelectList(db.User_Group.ToList(), "ID", "Name", user.GroupID);
            return PartialView(user);
        }
        [HttpPost]
        public ActionResult Create(User user)
        {
            ViewBag.GroupID = new SelectList(db.User_Group.ToList(), "ID", "Name");
            var session = (UserLogin)Session["DUY"];
            user.CreateBy = session.Name;
            user.CreateDate = DateTime.Now;
            user.PassWord = Encryptor.MD5Hash(user.PassWord);
            if (ModelState.IsValid)
            {

                if (db.Users.Count(d => d.Email == user.Email) > 0)
                {
                    SetAlert("Email đã có người sử dụng", "error");
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    SetAlert("Thêm tài khoản thành công", "success");
                    return RedirectToAction("Index");
                }

            }
            return View("Index");
        }
        [HttpPost]
        public ActionResult Edit(User entity)
        {
            ViewBag.GroupID = new SelectList(db.User_Group.ToList(), "ID", "Name", entity.GroupID);
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(entity.ID);
                var session = (UserLogin)Session["DUY"];
                entity.ModifiedBy = session.Name;


                if (db.Users.Count(d => d.Email == entity.Email) > 0 && user.Email != entity.Email)
                {
                    ModelState.AddModelError("", "Email Da co nguoi su dung");
                    SetAlert("Email đã có người sử dụng", "error");
                    return RedirectToAction("Index");
                }
                else
                {
                    if (session.Email == "admin@admin.com")
                    {
                        ChangeInfo(entity);
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        if (user.Email != session.Email)
                        {
                            SetAlert("Bạn không có quyền", "error");
                            return RedirectToAction("Index", "User");
                        }
                        else
                        {
                            ChangeInfo(entity);
                            return RedirectToAction("Index", "User");
                        }
                    }
                }

            }
            return  RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Delete(long id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            SetAlert("Xoa thanh cong", "error");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Status(long id)
        {
            var user = db.Users.Find(id);
            if (user.Status == true)
            {
                user.Status = false;
            }
            else user.Status = true;
            db.SaveChanges();
            SetAlert("Thay doi thanh cong!!", "error");
            return RedirectToAction("Index");
        }
      
        public void ChangeInfo(User entity)
        {
            User user = db.Users.Find(entity.ID);
            user.Name = entity.Name;
            if (!string.IsNullOrEmpty(entity.PassWord))
            {
                user.PassWord = Encryptor.MD5Hash(entity.PassWord);
            }
            else
            {
                user.PassWord = user.PassWord;
            }
            user.GroupID = entity.GroupID;
            user.Email = entity.Email;
            user.Phone = entity.Phone;
            user.Address = entity.Address;
            user.Status = entity.Status;
            user.ModifiedBy = entity.ModifiedBy;
            user.Vacxin = entity.Vacxin;
            user.ModifiedDate = DateTime.Now;
            db.SaveChanges();
            SetAlert("Sửa tài khoản thành công", "success");
        }
        public ActionResult Detail(long id)
        {
            User user = db.Users.Find(id);

            return PartialView(user);
        }
    }
}