using DCCovid.Common;
using DCCovid.Models;
using DCCovid.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private DCCovidDbcontext db = new DCCovidDbcontext();
        public ActionResult Index()
        {
            var sess = Session["MEMBER"] as UserLogin;
           
            if(sess !=null)
            {
                User user = db.Users.Find(sess.UserID);
                return View(db.Users.ToList().Where(d => d.Isdelete == false && d.ID != sess.UserID));
            }
            else
            {
                return Redirect("/home/Error");
            }
           
        }
        public ActionResult Login(string strURL)
        {
            Session.Add("url", strURL);
            return View();
        }
        public ActionResult Register()
        {
            ViewBag.SexID = new SelectList(db.Sexes.ToList(), "ID", "Name");
            ViewBag.CategoryID = new SelectList(db.CategoryUserPosts.ToList(), "ID", "Name");
            return View();
        }
        public ActionResult ConfirmRegister()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Remove("MEMBER");
            return Redirect("/");
        }
        public ActionResult ForgotPassWord()
        {
            return View();
        }
        public ActionResult EnterCode()
        {
            return View();
        }
        public ActionResult ChangePass(string code)
        {
            var user = db.Users.Single(d => d.Code == code);
            var sess = new UserLogin();
            sess.UserID = user.ID;
            sess.Email = user.Email;
            sess.Name = user.Name;
            Session.Add("Confirm", sess);
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = db.Users.SingleOrDefault(d => d.Email == model.Email);
                if (result == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại!");
                }
                else
                {
                    if (result.PassWord == Encryptor.MD5Hash(model.PassWord))
                    {
                        var userSession = new UserLogin();
                        userSession.Email = result.Email;
                        userSession.UserID = result.ID;
                        userSession.Name = result.Name;

                        if (result.Status == true)
                        {
                           
                            Session.Add("MEMBER", userSession);
                           
                            if (Session["url"] != null)
                            {
                                return Redirect(Session["url"].ToString());
                            }
                            else
                            {
                                return Redirect("/");
                            }

                        }
                        else
                        {
                            Session.Add("Confirm", userSession);
                            TempData["Active"] = "Vui lòng kích hoạt";
                            return RedirectToAction("ConfirmRegister", "User");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu sai!");
                    }
                }
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
           
            var list = db.Users.ToList();
            if (ModelState.IsValid)
            {
                if (list.Where(d => d.Email == model.Email).Count() > 0)
                {
                    ModelState.AddModelError("", "Email này đã có người sử dụng");
                }
                else
                {
                    var user = Create(model);

                    ViewBag.SendMail = "Vui lòng nhập mã xác nhận trong mail";
                    Mail(model.Name, user.Code, model.Email);
                    return RedirectToAction("ConfirmRegister", "User");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmRegister(string Code, string email)
        {
            if (ModelState.IsValid)
            {
                var sess = Session["Confirm"] as UserLogin; // = (UserLogin)Session["Confirm"]
                var user = new User();
                if (sess != null)
                {
                    user = db.Users.Single(d => d.Email == sess.Email);
                }
                else
                {
                    user = db.Users.Single(d => d.Email == email);
                }
                if (Code == user.Code)
                {
                    user.Status = true;
                    user.Code = Encryptor.MD5Hash(user.Code);
                    db.SaveChanges();
                    TempData["Success"] = "Thành công";
                    var mem = new UserLogin();
                    mem.UserID = sess.UserID;
                    mem.Email = sess.Email;
                    Session.Add("MEMBER", mem);
                    return RedirectToAction("UpdateSex","User");

                }
                else
                {
                    TempData["Error"] = " ko Thành công";
                }
            }

            return RedirectToAction("ConfirmRegister", "User");
        }
        public ActionResult UpdateSex()
        {
            ViewBag.SexID = new SelectList(db.Sexes.ToList(), "ID", "Name");
            ViewBag.cate = db.CategoryUserPosts.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult UpdateSex(User user)
        {
            var sess = Session["MEMBER"] as UserLogin;
            ViewBag.SexID = new SelectList(db.Sexes.ToList(), "ID", "Name");
            
            if (ModelState.IsValid)
            {
              
                if (sess != null)
                {
                  
                    var users = db.Users.Find(sess.UserID);
                    users.SexID = user.SexID;
                  
                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật thành công";
                    return Redirect("/Contact");
                }
                else
                {
                    ModelState.AddModelError("", "Bạn phải đăng nhập trước ");
                }

            }
            else
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi");
            }


            return View();
        }
        public ActionResult Like(long id)
        {
            var sess = Session["MEMBER"] as UserLogin;

            CategoryUserPost post = db.CategoryUserPosts.Find(id);
            User user = db.Users.Find(sess.UserID);
            post.Users.Add(user);
            db.SaveChanges();
            return Redirect("/User/UpdateSex");
        }
        public ActionResult DisLike(long id)
        {
            var sess = Session["MEMBER"] as UserLogin;

            CategoryUserPost post = db.CategoryUserPosts.Find(id);
            User user = db.Users.Find(sess.UserID);
            post.Users.Remove(user);
            db.SaveChanges();
            return Redirect("/User/UpdateSex");
        }
        [HttpPost]
        public ActionResult ForgotPassWord(User model)
        {
            var user = new User();
            user = db.Users.SingleOrDefault(d => d.Email == model.Email);
            if (user != null)
            {
                var userSession = new UserLogin();
                userSession.Email = user.Email;
                Session.Add("Confirm", userSession);
                string code = Random();
                code = Encryptor.MD5Hash(code);
                MailREset(user.Email, code, user.Email);
                user.Code = code;
                db.SaveChanges();
                ModelState.AddModelError("", "Chúng tôi đã gửi tin nhắn vào email của bạn! Vui lòng kiểm tra trong email");
            }
            else
            {
                ModelState.AddModelError("", "Email không trùng khớp");
            }
            return View();
        }

        [HttpPost]
        public ActionResult EnterCode(string Code, string UserName)
        {
            var sess = Session["Confirm"] as UserLogin; // = (UserLogin)Session["Confirm"]
            var user = new User();
            if (sess != null)
            {
                user = db.Users.Single(d => d.Email == sess.Email);
                if (Code == user.Code)
                {
                    return RedirectToAction("ChangePass", "User");
                }
                else
                {
                    ViewBag.Success = "Kích hoạt khong thành công";
                }
            }
            return RedirectToAction("EnterCode", "User");
        }

        [HttpPost]
        public ActionResult ChangePass(User model)
        {
            var user = new User();
            var sess = Session["Confirm"] as UserLogin; // = (UserLogin)Session["Confirm"]
            if (sess != null)
            {
                user = db.Users.Single(d => d.Email == sess.Email);
                user.PassWord = Encryptor.MD5Hash(model.PassWord);
                user.Code = null;
                db.SaveChanges();
                TempData["Success"] = "Đổi mật khẩu thành công";
                return Redirect("/");
            }
            return View();
        }
        public ActionResult ResendMail(string strURL)
        {
            var user = new User();
            var sess = Session["Confirm"] as UserLogin;
            if (sess != null)
            {
                user = db.Users.Single(d => d.Email == sess.Email);
                string code = Random();
                Mail(sess.Name, code, user.Email);
                user.Code = code;
                db.SaveChanges();
                ViewData["Success"] = "Đã gửi code";
                return Redirect(strURL);
            }
            return View();
        }

        public string Random()
        {
            Random rand = new Random();
            var numIterations = rand.Next(0, 999999);
            string x = numIterations.ToString("000000");
            return x;
        }
        public User Create(RegisterModel model)
        {
            var user = new User();
           
            user.PassWord = Encryptor.MD5Hash(model.PassWord);
            user.Name = model.Name;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Status = false;
            user.GroupID = "MEM";
            user.CategoryID = 1;
            user.SexID = 1;
            user.CreateDate = DateTime.Now;
            user.Code = Random();
            db.Users.Add(user);
            db.SaveChanges();
            var userSession = new UserLogin();
            userSession.Email = user.Email;
            userSession.UserID = user.ID;
            userSession.Name = user.Name;
            Session.Add("Confirm", userSession);
            return user;
        }
        public void Mail(string toEmail, string Content, string AddressEmail)
        {

            //gửi mail cho khách hàng 
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Mail/ConfirmMail.html"));

            content = content.Replace("{{CustomerName}}", toEmail);
            content = content.Replace("{{Code}}", Content);

            new Mail().SendMail(AddressEmail, "Xác nhận tài khoản từ DCDGear", content);
        }
        public void MailREset(string toEmail, string Content, string AddressEmail)
        {

            //gửi mail cho khách hàng 
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Mail/ResetPass.html"));

            content = content.Replace("{{CustomerName}}", toEmail);
            content = content.Replace("{{Code}}", Content);

            new Mail().SendMail(AddressEmail, "Lấy lại mật khẩu từ DCDGear", content);
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