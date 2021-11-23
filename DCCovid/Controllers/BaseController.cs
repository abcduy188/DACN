using DCCovid.Common;
using DCCovid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Controllers
{
    public class BaseController : Controller
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        protected void Load()
        {
            var sess = Session["MEMBER"] as UserLogin;
            if (sess != null)
            {
                var user = db.Users.Find(sess.UserID);
                var listroom = user.Rooms.ToList();
                var count = 0;
                var Listmess = new List<Message>();
                foreach (var i in listroom)
                {
                    Listmess = i.Messages.OrderByDescending(d => d.ID).Take(1).ToList();
                    if (Listmess.Count != 0)
                    {
                        var mess = Listmess.Last();

                        if (mess.Status == true && mess.UserID != sess.UserID)
                        {
                            count++;
                        }
                    }
                }
                var cou = new CountSes();
                cou.Count = count;
                Session.Add("CountSes", cou);

            }
        }
        protected void SetAlert(string message, string type)
        {   //temdata = viewbag
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
        }

    }
}