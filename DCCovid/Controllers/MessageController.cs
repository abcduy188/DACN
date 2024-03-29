﻿using DCCovid.Common;
using DCCovid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using PagedList;
using DCCovid.ViewModel;

namespace DCCovid.Controllers
{
    public class MessageController : BaseController
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index(int page = 1, int pageSize = 10)
        {

            var sessroom = Session["Room"] as RoomLogin;
            
            if (sessroom != null)
            {
                var room = db.Rooms.Find(sessroom.RoomID);
                if (room!= null)
                {
                    var messages = db.Messages.Include(m => m.Room).Include(m => m.User).Where(m => m.RoomID == room.ID).OrderByDescending(d => d.ID);
                
                    ViewBag.Mess = messages.ToList();
                }
                else
                {
                    SetAlert("Lỗi", "error");
                    return Redirect("/post");
                }

            }
            else
            {
                SetAlert("Lỗi", "error");
                return Redirect("/post");
            }
            List<Image> listimg = db.Images.Where(d => d.Type == "MESS").OrderBy(d => d.ID).ToList();
            ViewBag.ListImg = listimg;
            var sess = Session["MEMBER"] as UserLogin;

            if (sess != null)
            {
                User user = db.Users.Find(sess.UserID);
                var listroomofuser1 = user.Rooms.ToList();
                var listroomofuser2 = user.Rooms.Where(d => d.Name.StartsWith("Find@@@")).ToList();
                var listroomofuser = listroomofuser1.Except(listroomofuser2);
                var listuser = new List<User>();
                var listuser2 = new List<User>();
                var user2 = new User();
                foreach (var i in listroomofuser)
                {
                    if (i.Messages.Count > 0)
                    {
                        listuser2 = i.Users.Where(d => d.ID != user.ID).ToList();
                        user2 = db.Users.Find(listuser2.Last().ID);
                        listuser.Add(user2);
                    }
                }

                ViewBag.User = listuser;
            }
            Load();
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(MessageViewModel mesview)
        {
            var message = new Message();
            message.Text = mesview.Text;
            message.TypeID = mesview.TypeID;
            var sess = Session["MEMBER"] as UserLogin;
            var user = db.Users.Find(sess.UserID);
            Room room = new Room();
            if (message.TypeID == 1 && user.Iscouple == 1)
            {
                if (message.Text == "Bắt đầu !!!")
                {
                    var demo = GiveMeANumber(mesview.Fav);
                    if (demo == -1)
                    {
                        SetAlert("Hiện không có đối phuọng, bạn thử lại sau nhé", "error");
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    User user2 = db.Users.Find(demo);
                    room.Name = ("Find@@@" + user.ID + user2.ID).ToString();
                    string namechange = ("Find@@@" + user2.ID + user.ID).ToString();
                    if (db.Rooms.Count(d => d.Name == room.Name) > 0 || db.Rooms.Count(d => d.Name == namechange) > 0)
                    {
                        var sessroom = new RoomLogin();
                        Room room1 = db.Rooms.SingleOrDefault(d => d.Name == namechange);
                        if (room1 == null)
                        {
                            room1 = db.Rooms.SingleOrDefault(d => d.Name == room.Name);
                        }
                        sessroom.RoomID = room1.ID;
                        sessroom.Name = room1.Name;
                        Session.Add("Room", sessroom);

                        return Redirect("/Message/Index");
                    }
                    else
                    {
                        user.Iscouple = 2;
                        user2.Iscouple = 2;
                        db.Rooms.Add(room);
                        db.SaveChanges();
                        var sessroom = new RoomLogin();
                        Room room1 = db.Rooms.Find(room.ID);
                        sessroom.RoomID = room.ID;
                        sessroom.Name = room.Name;
                        Session.Add("Room", sessroom);
                        room.Users.Add(user);
                        room.Users.Add(user2);
                        db.SaveChanges();
                        SetAlert("Đã tìm được đối tượng", "success");
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    SetAlert("Định dạng không đúng", "success");
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {

                var sessroom = Session["Room"] as RoomLogin;
                if (sessroom == null)
                {
                    SetAlert("Bạn đã có đối tượng nhắn tin rồi", "warning");
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
                room = db.Rooms.Find(sessroom.RoomID);
                if (ModelState.IsValid)
                {
                    message.RoomID = room.ID;
                    message.UserID = user.ID;
                    message.Status = true;
                    message.CreateDate = DateTime.Now;

                    db.Messages.Add(message);

                    db.SaveChanges();
                    room.LastMess = message.ID;

                    db.SaveChanges();
                    HttpFileCollectionBase files = Request.Files; //lấy file 
                    for (int i = 0; i < files.Count; i++) //đi qua lần lượt các file => lưu file
                    {
                        HttpPostedFileBase fileUpload = files[i];
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        if (fileName != "")
                        {

                            var path = Path.Combine(Server.MapPath("~/Assets/Thumbnails/"), fileName);
                            fileUpload.SaveAs(path);
                            InsertIMG(message.ID, fileName); // gọi hàm thêm ảnh, truyền vào id và tên ảnh
                        }

                    }
                    return RedirectToAction("Index");
                }
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
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
        public void InsertIMG(long id, string fileName)
        {
            var img = new Image();
            img.FileName = fileName;
            img.Type = "MESS";
            img.TypeID = id;
            db.Images.Add(img);
            db.SaveChanges();
        }
        [HttpPost]
        public ActionResult FindFr(long id)
        {

            User emp = db.Users.Where(x => x.ID == id).FirstOrDefault<User>();
            if (emp.Iscouple == 0)
                emp.Iscouple = 1;
            else if (emp.Iscouple == 1)
            {
                emp.Iscouple = 2;
            }
            else
            {
                emp.Iscouple = 0;
            }

            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully", JsonRequestBehavior.AllowGet });

        }
        private long GiveMeANumber(long? a)
        {
            var sess = Session["MEMBER"] as UserLogin;
            User user = db.Users.Find(sess.UserID);
            ArrayList ar = new ArrayList();

            if (a == null || a == 1)
            {
                foreach (var i in db.Users.Where(d => d.ID != sess.UserID && d.Isdelete == false && d.Iscouple == 1).ToList())
                {
                    ar.Add(i.ID);
                }
                if (ar.Count != 0)
                {
                    Random r = new Random();
                    int index = r.Next(0, ar.Count - 1);
                    return (long)ar[index];
                }
            }
            else
            {
                var cate = db.CategoryUserPosts.Find(a);
                var list = cate.Users.ToList();
                foreach (var i in list.Where(d => d.ID != sess.UserID && d.Isdelete == false && d.Iscouple == 1).ToList())
                {
                    ar.Add(i.ID);
                }
                if (ar.Count != 0)
                {
                    Random r = new Random();
                    int index = r.Next(0, ar.Count - 1);
                    return (long)ar[index];
                }
            }

            return -1;

        }

    }
}