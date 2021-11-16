using DCCovid.Common;
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

namespace DCCovid.Controllers
{
    public class MessageController : Controller
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index()
        {
            var sessroom = Session["Room"] as RoomLogin;
            var room = db.Rooms.Find(sessroom.RoomID);
            var messages = db.Messages.Include(m => m.Room).Include(m => m.User).Where(m => m.RoomID == room.ID);
            ViewBag.Mess = messages.ToList();
            List<Image> listimg = db.Images.Where(d => d.Type == "MESS").OrderBy(d => d.ID).ToList();
            ViewBag.ListImg = listimg;
            return View();
        }

        // GET: Messages/Details/5

        public ActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RoomID,UserID,Text")] Message message)
        {
            var sess = Session["MEMBER"] as UserLogin;
            var sessroom = Session["Room"] as RoomLogin;
            var user = db.Users.Find(sess.UserID);
            var room = db.Rooms.Find(sessroom.RoomID);
            if (ModelState.IsValid)
            {
                message.RoomID = room.ID;
                message.UserID = user.ID;
                message.CreateDate = DateTime.Now;
                db.Messages.Add(message);
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
    }
}