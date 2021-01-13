using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DbTables.Controllers
{
    public class SysCodeController : Controller
    {
        // GET: SysCode
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult qrcode()
        {
            return View();
        }
    }
}