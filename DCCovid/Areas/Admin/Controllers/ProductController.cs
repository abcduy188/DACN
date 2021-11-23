using DCCovid.Models;
using DCCovid.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCCovid.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private DCCovidDbcontext db = new DCCovidDbcontext();
        // GET: Admin/Product
        public ActionResult Index()
        {
            return View();
        }
    }
}