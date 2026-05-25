using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PTDA_Demo.Models;

namespace PTDA_Demo.Controllers
{
    public class UserController : Controller
    {
        GreenMartDBContext db = new GreenMartDBContext();
        // GET: User

        public ActionResult Homepage()
        {
            return Content("Chưa code");
        }

        [HttpGet]
        public ActionResult Profile(int? id)
        {
            if(id==0 && Session["UserID"]!=null)
            {
                id = (int)Session["UserID"];
            }    
            if(id==null)
            {
                return RedirectToAction("Login", "Admin");
            }
            var userProfile = db.Users.SingleOrDefault(u => u.UserID == id);
            if(userProfile==null)
            {
                ViewBag.Error = "Không thể tải thông tin lúc này. Vui lòng thử lại sau.";
                return View();
            }
            return View(userProfile);
        }

    }
}