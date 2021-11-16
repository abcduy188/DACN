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
    public class RoomController : Controller
    {

        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index()
        {
            return View(db.Rooms.ToList());
        }


        [HttpPost]
        public ActionResult Create(Room room, long? id)
        {
            if (ModelState.IsValid)
            {
                var sess = Session["MEMBER"] as UserLogin;
                User user = db.Users.Find(sess.UserID);
                User user2 = db.Users.Find(id);
                room.Name = (user.Name + user2.Name).ToString();
                string namechange = (user2.Name + user.Name).ToString();
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
                    return Redirect("/Message/Index");
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