using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using My_Project.Models;

namespace My_Project.Controllers
{
    public class ShopingCartController : Controller
    {
        Factory_vtEntities db = new Factory_vtEntities();

        public ShoppingCart bring_cart()
        {
            var my_scart = (ShoppingCart)Session["myscart"];
            //ShoppingCart my_scart = (ShoppingCart)Session["myscart"];
            if (my_scart == null)
            {
                my_scart = new ShoppingCart();
                Session["myscart"] = my_scart;
            }
            return my_scart;
        }
        
        public ActionResult Index(string mess)
        {
            List<SelectListItem> perr_list = new List<SelectListItem>();
            var list = db.Personnel.Where(x => x.depart_id == 5).ToList();
            foreach(var item in list)
            {
                var _perr = new SelectListItem
                {
                    Text = item.per_name.ToString(),
                    Value = item.per_id.ToString(),
                };
                perr_list.Add(_perr);
            }
            ViewBag.perr = perr_list;
            ViewBag.mess = mess;
            return View(bring_cart());
        }

        /*public async Task<ActionResult> add_carts(int? pro_id, int? quantity)
        {
            var _quantity = quantity ?? 0;
            var my_product = db.Product.FirstOrDefault(x => x.product_id == pro_id);
            if(my_product != null)
            {
                bring_cart().add_cart(my_product, _quantity);
            }
            return RedirectToAction("Index");
        }*/

        /*[HttpPost]*/
        public async Task<ActionResult> add_carts(int? pro_id, int? quantity, int? perr)
        {
            var _quantity = quantity ?? 0;
            var _perr = db.Personnel.FirstOrDefault(x => x.per_id == perr);
            Session["perrso"] = _perr;
            var my_product = db.Product.FirstOrDefault(x => x.product_id == pro_id);
            string mess = "";
            if (my_product != null)
            {
                bring_cart().add_cart(my_product, _quantity);
                /*mess = "Pls Choose Someone";*/
            }
            /*if (perr != null)
                return RedirectToAction("SalesIndex", "Sales");*/
            /*return RedirectToAction("Index", new { mess = mess });*/
            return RedirectToAction("SalesIndex", "Sales");
        }

        public async Task<ActionResult> delete_cart(int? id)
        {
            var my_product = db.Product.FirstOrDefault(x => x.product_id == id);
            if (my_product != null)
            {
                bring_cart().delete_cart(my_product);
            }
            return RedirectToAction("Index");
        }

        public ActionResult clear_cart()
        {
            bring_cart().clear_cart();
            return RedirectToAction("Index");
        }

        public ActionResult back()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}