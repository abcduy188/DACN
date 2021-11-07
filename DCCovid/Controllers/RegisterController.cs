using DCCovid.Common;
using DCCovid.Models;
using DCCovid.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
          
            ViewBag.Cate = new SelectList(db.CategoryUsers.ToList(), "ID", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {

            
            return View();
        }
        public User Create(User model)
        {
            var user = new User();
            user.PassWord = Encryptor.MD5Hash(model.PassWord);
            user.Name = model.Name;
            user.Email = model.Email;
            user.Phone = model.Phone;
            //user.CategoryUsers = model.CategoryUsers;
            user.Status = false;
            user.GroupID = "MEM";
            user.CreateDate = DateTime.Now;
            db.Users.Add(user);
            db.SaveChanges();
            var userSession = new UserLogin();
            userSession.Email = user.Email;
            Session.Add("Confirm", userSession);
            return user;
        }

    }
}