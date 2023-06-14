using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using My_Project.Models;
namespace My_Project.Controllers
{
    public class HomeController : Controller
    {
        Factory_vtEntities db = new Factory_vtEntities();
        public ActionResult Index(string login_message)
        {
            ViewBag.loginmessage = login_message;
            return View();
        }

        [ChildActionOnly]
        public async Task<ActionResult> AllCategories()
        {
            var categories = db.Category.OrderBy(x => x.cate_name).ToList();
            return PartialView(categories);
        }

        [ChildActionOnly]
        public async Task<ActionResult> Login()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {      
            var cust = db.Customer.FirstOrDefault(x => x.customer_username == username && x.customer_password == password);
            var per = db.Personnel.FirstOrDefault(x => x.per_username == username && x.per_password == password);
            string login_message = "";

            if (cust != null)
            {
                Session["mycust"] = cust;
                login_message = "Hello" + cust.customer_name;
            }
            else if(per !=null)
            {
                Session["myuser"] = per;
                login_message = "Hello" + per.per_name;
            }
            else if (username == "admin" && password == "admin123")
            {
                Session["myuser"] = "Admin";
                login_message = "Hello Admin";
            }
            else
            {
                login_message = "Wrong Information Try Again";
            }
            return RedirectToAction("Index", new { login_message });
        }

        public ActionResult SignOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            string login_message = "Logged Out";
            return RedirectToAction("Index", new { login_message });
        }
    }
}