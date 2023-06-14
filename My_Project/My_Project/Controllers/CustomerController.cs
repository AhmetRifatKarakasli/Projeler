using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using My_Project.Models;

namespace My_Project.Controllers
{
    public class CustomerController : Controller
    {
        private Factory_vtEntities db = new Factory_vtEntities();

        public async Task<ActionResult> Index()
        {
            var customer = db.Customer.Include(c => c.Country);
            return View(await customer.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customer.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        public ActionResult Create()
        {
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "customer_id,customer_name,customer_address,country_id,customer_phone,customer_email,customer_username,customer_password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customer.Add(customer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name", customer.country_id);
            return View(customer);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customer.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name", customer.country_id);
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "customer_id,customer_name,customer_address,country_id,customer_phone,customer_email,customer_username,customer_password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name", customer.country_id);
            return View(customer);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customer.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Customer customer = await db.Customer.FindAsync(id);
            db.Customer.Remove(customer);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task<ActionResult> Listing()
        {
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name");

            var customers = db.Customer.OrderBy(x => x.customer_name).ToList();
            return View(customers);
        }

        [HttpPost]
        public async Task<ActionResult> Listing(int? country_id)
        {
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name");

            IEnumerable<Customer> mycustomer;

            if (country_id != null)//şehir
                mycustomer = db.Customer.Where(x => x.country_id == country_id).OrderBy(x => x.customer_name);
            else//hepsi
                mycustomer = db.Customer.OrderBy(x => x.customer_name).ToList();
            return View(mycustomer);
        }

        public async Task<ActionResult> CustEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customer.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name", customer.country_id);
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CustEdit(Customer customer)
        {
            string mess = "";
            if (ModelState.IsValid)
            {
                mess = "Changed";
                db.Entry(customer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return View(customer);
            }
            else
                mess = "Not Changed";
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name", customer.country_id);
            return View(customer);
        }
    }
}
