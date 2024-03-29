﻿using DCCovid.Common;
using DCCovid.Models;
using DCCovid.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        private DCCovidDbcontext db = new DCCovidDbcontext();

        [ChildActionOnly]
        public PartialViewResult Index()
        {
            var sess = Session["MEMBER"] as UserLogin;
            Load();
            if (sess != null)
            {
                User user = db.Users.Find(sess.UserID);
                ViewBag.ListPost = db.PostCMTs.ToList();
                return PartialView(db.Users.ToList().Where(d => d.Isdelete == false && d.ID != sess.UserID));
            }
            else
            {
                return PartialView(db.Users.ToList().Where(d => d.Isdelete == false));
            }

        }
        [ChildActionOnly]
        public PartialViewResult Message()
        {
            var sess = Session["MEMBER"] as UserLogin;
            ViewBag.listCate = db.CategoryUserPosts.ToList();
            ViewBag.listUser = db.Users.Where(d => d.ID != sess.UserID && d.Isdelete == false && d.Status == true).ToList();
            Load();
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

                return PartialView(listuser);
            }
            else
                return PartialView();
        }
        public ActionResult GetData()
        {
            var sess = Session["MEMBER"] as UserLogin;
            int ses = 0;
            List<User> results = new List<User>();
            db.Configuration.ProxyCreationEnabled = false;
            if (sess != null)
            {
                results = db.Users.Where(d => d.Isdelete == false && d.ID != sess.UserID).ToList();
                ses = 1;
            }
            else
            {
                results = db.Users.Where(d => d.Isdelete == false).ToList();
            }
            return Json(new { Data = results, TotalItems = results.Count, Ses = ses }, JsonRequestBehavior.AllowGet);
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
                    return RedirectToAction("UpdateSex", "User");

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
        public ActionResult Like(long id, string strURL)
        {
            var sess = Session["MEMBER"] as UserLogin;

            CategoryUserPost post = db.CategoryUserPosts.Find(id);
            var listcate = db.CategoryUserPosts.Where(d => d.ID != post.ID).ToList();
            User user = db.Users.Find(sess.UserID);
            if (post.ID == 1)
            {
                foreach (var item in listcate)
                {
                    item.Users.Add(user);

                }
                post.Users.Add(user);
                db.SaveChanges();
            }
            else
            {
                post.Users.Add(user);
                db.SaveChanges();
            }
            if (strURL != null)
            {
                return Redirect(strURL);
            }
            return Redirect("/User/UpdateSex");
        }
        public ActionResult DisLike(long id, string strURL)
        {
            var sess = Session["MEMBER"] as UserLogin;

            CategoryUserPost post = db.CategoryUserPosts.Find(id);
            CategoryUserPost post1 = db.CategoryUserPosts.Find(1);
            var listcate1 = db.CategoryUserPosts.Where(d => d.ID != post1.ID).ToList();
            var listcate = db.CategoryUserPosts.Where(d => d.ID != post.ID).ToList();
            User user = db.Users.Find(sess.UserID);
            if (post.ID == 1)
            {
                foreach (var item in listcate)
                {
                    item.Users.Remove(user);

                }
                post.Users.Remove(user);
                db.SaveChanges();
            }
            else
            {
                post.Users.Remove(user);
                foreach (var i in listcate1)
                {
                    if (i.ID == user.ID)
                        post1.Users.Remove(user);
                }
                db.SaveChanges();
            }
            if (strURL != null)
            {
                return Redirect(strURL);
            }
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
            user.BirthDay = model.BirthDay;
            user.CreateDate = DateTime.Now;
            user.Code = Random();
            user.Iscouple = 0;
            user.Isdelete = false;
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


        public ActionResult getUser()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var sess = Session["MEMBER"] as UserLogin;
            var user = db.Users.Find(sess.UserID);

            return Json(new { data = user }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Info(long id)
        {
            var sess = Session["MEMBER"] as UserLogin;
            Load();

            if (id == sess.UserID)
            {

                var user = db.Users.Find(id);
                return View();
            }
            else
            {
                SetAlert("Lỗi", "error");
                return Redirect("/");
            }
        }
        public ActionResult ChangeInfo(long id)
        {
            Load();
            var sess = Session["MEMBER"] as UserLogin;
            if (id == sess.UserID)
            {
                var user = db.Users.Find(id);
                ViewBag.SexID = new SelectList(db.Sexes.ToList(), "ID", "Name", user.SexID);
                ViewBag.cate = db.CategoryUserPosts.ToList();
                return View(user);
            }
            else
            {
                SetAlert("Lỗi", "error");
                return Redirect("/");
            }
        }
        [HttpPost]
        public ActionResult ChangeInfo(User model)
        {
            var user = db.Users.Find(model.ID);
            if (ModelState.IsValid)
            {
                ViewBag.SexID = new SelectList(db.Sexes.ToList(), "ID", "Name", user.SexID);

                user.Name = model.Name;
                user.Address = model.Address;
                user.Phone = model.Phone;
                user.SexID = model.SexID;
                if (model.BirthDay != null)
                {
                    user.BirthDay = model.BirthDay;

                    if (model.BirthDay >= DateTime.Now)
                    {
                        SetAlert("Ngày sinh không được lớn hơn ngày hiện tại", "error");
                        return View();
                    }
                }
                HttpFileCollectionBase files = Request.Files; //lấy file 
                for (int i = 0; i < files.Count; i++) //đi qua lần lượt các file => lưu file
                {
                    HttpPostedFileBase fileUpload = files[i];
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    if (fileName != "")
                    {
                        var path = Path.Combine(Server.MapPath("~/Assets/Thumbnails/"), fileName);
                        fileUpload.SaveAs(path);
                        user.ImageUser = fileName; // gọi hàm thêm ảnh, truyền vào id và tên ảnh
                    }
                }
                db.SaveChanges();
                SetAlert("Cập nhật thông tin thành công", "success");

            }
            else
            {
                SetAlert("Cập nhật thông tin thất bại", "error");
                return View();
            }
            string red = "/user/info/" + user.ID;
            return Redirect(red);
        }

        [HttpPost]
        public ActionResult ChangePassWord(CPWViewModel model)
        {
            var sess = Session["MEMBER"] as UserLogin;
            var user = db.Users.Find(sess.UserID);

            if (user.PassWord != Encryptor.MD5Hash(model.OldPassWord))
            {
                SetAlert("Mật khẩu cũ không đúng", "success");
                return Json(new { success = false, message = "Passwordold not match", JsonRequestBehavior.AllowGet });
            }
            else if (model.NewPassWord != model.ConfirmPassWord)
            {

                SetAlert("Nhập lại mật khẩu không đúng", "success");
                return Json(new { success = false, message = "Passwordold not match", JsonRequestBehavior.AllowGet });
            }
            else
            {
                user.PassWord = Encryptor.MD5Hash(model.NewPassWord);
                db.SaveChanges();
                SetAlert("Đổi mật khẩu thành công", "success");
                return Json(new { success = true, message = "Updated Successfully", JsonRequestBehavior.AllowGet });
            }

        }
        public void InsertIMG(long id, string fileName)
        {
            var img = new Image();
            img.FileName = fileName;
            img.Type = "USER";
            img.TypeID = id;
            db.Images.Add(img);
            db.SaveChanges();
        }
        public new ActionResult Profile(long id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                SetAlert("Khong co nguoi ", "error");
                return Redirect("/post");
            }
            List<Image> listimg = db.Images.Where(d => d.Type == "POST").OrderBy(d => d.ID).ToList();
            ViewBag.ListImg = listimg;
            ViewBag.cmt = db.PostCMTs.Where(d => d.IsDelete == false && d.PostID != null).ToList();
            ViewBag.listpost = db.PostCMTs.Where(d => d.IsDelete != true && d.PostID == null && d.UserID == id && d.Status == 1).ToList();
            ViewBag.CateID = new SelectList(db.CategoryUserPosts.ToList(), "ID", "Name");
            return View(user);

        }
    }
}