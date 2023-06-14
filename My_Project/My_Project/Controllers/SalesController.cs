using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using My_Project.Models;
namespace My_Project.Controllers
{
    public class SalesController : Controller
    {
        Factory_vtEntities db = new Factory_vtEntities();

        // GET: Sales
        public ActionResult SalesIndex()
        {
            var pers = (Personnel)Session["perrso"];
            var cust = (Customer)Session["mycust"];
            var mycart = (ShoppingCart)Session["myscart"];
            var new_sales_number = db.Sales.Max(x => x.sale_number);
            new_sales_number += 1;
            string mess = "";

            if(pers == null)
            {
                mess = "Pls choose personnel";
            }
            else if(cust == null)
            {
                mess = "Pls sign in";
            }
            else
            {
                foreach(var item in mycart.Myshopingcart)
                {
                    /*Sales new_sales = new Sales()
                    {
                        sale_number = new_sales_number,
                        product_id = item.prdct.product_id,
                        customer_id = cust.customer_id,
                        sale_date = DateTime.Now,
                        sale_quantity = Convert.ToInt32(item.quantity),
                        per_id = item.prsnnl.per_id
                    };*/
                    Sales new_sales = new Sales();
                    new_sales.sale_number = new_sales_number;
                    new_sales.product_id = item.prdct.product_id;
                    new_sales.customer_id = cust.customer_id;
                    new_sales.sale_date = DateTime.Now;
                    new_sales.sale_quantity = Convert.ToInt32(item.quantity);
                    new_sales.per_id = pers.per_id;
                }
                mess = "Your sales number: " + new_sales_number;
            }
            return RedirectToAction("Index", "ShopingCart", new { mess = mess });
        }

        public async Task<ActionResult> Listing()
        {
            List<SelectListItem> perr_list = new List<SelectListItem>();
            var list = db.Personnel.Where(x => x.depart_id == 5).ToList();
            foreach (var item in list)
            {
                var _perr = new SelectListItem
                {
                    Text = item.per_name.ToString(),
                    Value = item.per_id.ToString(),
                };
                perr_list.Add(_perr);
            }
            ViewBag.perr = perr_list;
            var sales = db.Sales.GroupBy(x => x.sale_number);
            return View(sales);
        }

        [HttpPost]
        public async Task<ActionResult> Listing(DateTime? date1, DateTime? date2, int? perr)
        {
            List<SelectListItem> perr_list = new List<SelectListItem>();
            var list = db.Personnel.Where(x => x.depart_id == 5).ToList();
            foreach (var item in list)
            {
                var _perr = new SelectListItem
                {
                    Text = item.per_name.ToString(),
                    Value = item.per_id.ToString(),
                };
                perr_list.Add(_perr);
            }
            ViewBag.perr = perr_list;

            IQueryable mysales;

            if (date1 != null && date2 != null && perr != null)// Hepsi seçili
                mysales = db.Sales.Where(x => x.sale_date >= date1 && x.sale_date <= date2 && x.per_id == perr).GroupBy(x => x.sale_number);
            else if (date1 != null && date2 != null && perr == null)// Tarih 1 ve 2 seçili
                mysales = db.Sales.Where(x => x.sale_date >= date1 && x.sale_date <= date2).GroupBy(x => x.sale_number);
            else if (date1 != null && date2 == null && perr != null)// Tarih 1 ve perr seçili
                mysales = db.Sales.Where(x => x.sale_date >= date1 && x.per_id == perr).GroupBy(x => x.sale_number);
            else if (date1 == null && date2 != null && perr != null)// Tarih 2 ve perr seçili
                mysales = db.Sales.Where(x => x.sale_date <= date2 && x.per_id == perr).GroupBy(x => x.sale_number);
            else if (date1 != null && date2 == null && perr == null)// Tarih 1
                mysales = db.Sales.Where(x => x.sale_date >= date1).GroupBy(x => x.sale_number);
            else if (date1 == null && date2 != null && perr == null)// Tarih 2
                mysales = db.Sales.Where(x => x.sale_date <= date2).GroupBy(x => x.sale_number);
            else if (date1 == null && date2 == null && perr != null)// Perr
                mysales = db.Sales.Where(x => x.per_id == perr).GroupBy(x => x.sale_number);
            else// Hepsini Göster
                mysales = db.Sales.GroupBy(x => x.sale_number);
            return View(mysales);
        }
    }
}