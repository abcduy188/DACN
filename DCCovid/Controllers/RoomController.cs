using DCCovid.Common;
using DCCovid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Controllers
{
    public class RoomController : BaseController
    {

        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index()
        {
            return View(db.Rooms.ToList());
        }
        public ActionResult CancelChat()
        {

            var sess = Session["MEMBER"] as UserLogin;
            User user = db.Users.Find(sess.UserID);
            var listroom = user.Rooms.ToList();
            var roomd = new Room();
            foreach (var item in listroom)
            {

                if (item.Name.StartsWith("Find@@@") == true)
                {
                    roomd = db.Rooms.Find(item.ID);

                }
            }
            if (roomd.ID == 0)
            {
                user.Iscouple = 0;
                db.SaveChanges();
                Session.Remove("messses");
                return Json(new { success = true, message = "Changed Successfully", JsonRequestBehavior.AllowGet });
            }
            else
            {
                foreach (var i in roomd.Messages.ToList())
                {
                    db.Messages.Remove(i);
                    db.SaveChanges();
                }
                foreach (var u in roomd.Users.ToList())
                {
                    u.Iscouple = 0;
                    db.SaveChanges();
                }
                db.Rooms.Remove(roomd);
                user.Iscouple = 0;
                db.SaveChanges();
                Session.Remove("messses");
                return Json(new { success = false, message = "Changed Successfully", JsonRequestBehavior.AllowGet });
            }
           

        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Room room, long? id)
        {

            if (ModelState.IsValid)
            {
                var sess = Session["MEMBER"] as UserLogin;
                if(sess!= null)
                {
                    var cou = new CountSes();
                    var count = Session["CountSes"] as CountSes;
                    int counttime = count.Count;
                    var sessroom = new RoomLogin();

                    User user = db.Users.Find(sess.UserID);
                    User user2 = db.Users.Find(id);
                    if (user2 == null)
                    {
                        var listroom = user.Rooms.ToList();
                        var roomd = new Room();
                        foreach (var item in listroom)
                        {

                            if (item.Name.StartsWith("Find@@@") == true)
                            {
                                roomd = db.Rooms.Find(item.ID);
                            }
                        }

                        sessroom.RoomID = roomd.ID;
                        sessroom.Name = roomd.Name;
                        Session.Add("Room", sessroom);
                        return Redirect("/Message/Index");
                    }
                    else
                    {


                        room.Name = (user.ID + user2.ID).ToString();

                        string namechange = (user2.ID + user.ID).ToString();
                        if (db.Rooms.Count(d => d.Name == room.Name) > 0 || db.Rooms.Count(d => d.Name == namechange) > 0)
                        {

                            Room room1 = db.Rooms.SingleOrDefault(d => d.Name == namechange);
                            if (room1 == null)
                            {
                                room1 = db.Rooms.SingleOrDefault(d => d.Name == room.Name);
                            }
                            var roomm = db.Rooms.Find(room1.ID);
                            var mes = db.Messages.Find(roomm.LastMess);
                            if (mes != null)
                            {
                                mes.Status = false;
                                counttime--;
                                cou.Count = counttime;
                                Session.Add("CountSes", cou);
                                db.SaveChanges();
                            }

                            sessroom.RoomID = room1.ID;
                            sessroom.Name = room1.Name;
                            Session.Add("Room", sessroom);
                            return Redirect("/Message/Index");
                        }
                        else
                        {
                            db.Rooms.Add(room);
                            db.SaveChanges();

                            Room room1 = db.Rooms.Find(room.ID);
                            sessroom.RoomID = room.ID;
                            sessroom.Name = room.Name;
                            Session.Add("Room", sessroom);

                            room.Users.Add(user);
                            room.Users.Add(user2);
                            db.SaveChanges();
                            return Redirect("/Message/Index");
                        }

                    }

                }
                else
                {
                    SetAlert("Đã xảy ra lỗi", "error");
                    return Redirect("/user/login");
                }

            }
            return View(room);
        }



        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Room room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
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
    }
}