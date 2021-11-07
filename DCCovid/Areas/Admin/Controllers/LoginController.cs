using DCCovid.Common;
using DCCovid.Models;
using DCCovid.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
       
        // GET: Admin/Login
        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Remove("DUY");
            return RedirectToAction("Index", "Login");
        }
        public ActionResult Login(LoginModel model)
        {
            string abc = Encryptor.MD5Hash(model.PassWord);
            if (ModelState.IsValid)
            {
                var result = db.Users.SingleOrDefault(d => d.Email == model.Email);
                if (result == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại!");
                }
                else
                {
                    if (result.GroupID != "ADMIN" && result.GroupID != "MOD")
                    {
                        ModelState.AddModelError("", "Bạn không có quyền đăng nhập");
                    }
                    else
                    {
                        if (result.Status == false)
                        {
                            ModelState.AddModelError("", "Tài khoản đang bị khóa!");
                        }
                        else
                        {
                            if (result.PassWord == Encryptor.MD5Hash(model.PassWord))
                            {
                                var userSession = new UserLogin();
                                userSession.Email = result.Email;
                                userSession.UserID = result.ID;
                                userSession.Name = result.Name;
                                if (result.GroupID == "ADMIN")
                                {
                                    Session.Add("DUY", userSession);
                                }
                                else
                                {
                                    Session.Add("MOD", userSession);
                                }
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Mật khẩu sai!");
                            }
                        }
                    }

                }
            }
            return View("Index");
        }
    }
}