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
using System.IO;
using PagedList;

namespace My_Project.Controllers
{
    public class ProductController : Controller
    {
        private Factory_vtEntities db = new Factory_vtEntities();

        public async Task<ActionResult> Index()
        {
            var product = db.Product.Include(p => p.Category).Include(p => p.Supplier);
            return View(await product.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Product.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Create()
        {
            ViewBag.cate_id = new SelectList(db.Category, "cate_id", "cate_name");
            ViewBag.supp_id = new SelectList(db.Supplier, "supp_id", "supp_name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(/*[Bind(Include = "product_id,product_name,product_image,cate_id,product_price,supp_id")] Product product*/Product new_product, HttpPostedFileBase file)
        {
            /*if (ModelState.IsValid)
            {
                db.Product.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.cate_id = new SelectList(db.Category, "cate_id", "cate_name", product.cate_id);
            ViewBag.supp_id = new SelectList(db.Supplier, "supp_id", "supp_name", product.supp_id);
            return View(product);*/
            ViewBag.cate_id = new SelectList(db.Category, "cate_id", "cate_name");
            ViewBag.supp_id = new SelectList(db.Supplier, "supp_id", "supp_name");
            string file_name = "resimyok.jpg";

            if (ModelState.IsValid)
            {
                if (file != null)//resim geldiyse
                {
                    file_name = Path.GetFileName(file.FileName);
                    string extension = Path.GetExtension(file_name);
                    if (extension == ".jpg" && extension == ".png")
                    {
                        string path = Path.Combine(Server.MapPath("~/Products_images"), file_name);
                        file.SaveAs(path);
                        new_product.product_image = file_name;
                        db.Product.Add(new_product);
                        db.SaveChanges();
                        ViewBag.imagemessage = "Image Uploaded";
                    }
                    else
                        ViewBag.imagemessage = "Pls Choose Image";
                }
                else
                {
                    new_product.product_image = file_name;
                    db.Product.Add(new_product);
                    db.SaveChanges();
                    ViewBag.imagemessage = "Uploaded Without Image";
                }
            }

            return View(new_product);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Product.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.cate_id = new SelectList(db.Category, "cate_id", "cate_name", product.cate_id);
            ViewBag.supp_id = new SelectList(db.Supplier, "supp_id", "supp_name", product.supp_id);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "product_id,product_name,product_image,cate_id,product_price,supp_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.cate_id = new SelectList(db.Category, "cate_id", "cate_name", product.cate_id);
            ViewBag.supp_id = new SelectList(db.Supplier, "supp_id", "supp_name", product.supp_id);
            return View(product);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Product.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Product.FindAsync(id);
            db.Product.Remove(product);
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
            ViewBag.cate_id = new SelectList(db.Category, "cate_id", "cate_name");
            ViewBag.supp_id = new SelectList(db.Supplier, "supp_id", "supp_name");

            var products = db.Product.OrderBy(x => x.product_name).ToList();
            return View(products);
        }

        [HttpPost]
        public async Task<ActionResult> Listing(int? cate_id, int? supp_id)
        {
            ViewBag.cate_id = new SelectList(db.Category, "cate_id", "cate_name");
            ViewBag.supp_id = new SelectList(db.Supplier, "supp_id", "supp_name");

            IEnumerable<Product> myproduct;

            if (cate_id != null && supp_id != null)//cate ve supp
                myproduct = db.Product.Where(x => x.cate_id == cate_id && x.supp_id == supp_id).OrderBy(x => x.product_name);
            else if (cate_id != null && supp_id == null)//cate
                myproduct = db.Product.Where(x => x.cate_id == cate_id).OrderBy(x => x.product_name);
            else if (cate_id == null && supp_id != null)//supp
                myproduct = db.Product.Where(x => x.supp_id == supp_id).OrderBy(x => x.product_name);
            else
                myproduct = db.Product.OrderBy(x => x.product_name).ToList();
            return View(myproduct);
        }

        public async Task<ActionResult> Pages(int? id, int? page)//ürünleri sayfada gösterecek
        {
            var page_no = page ?? 1;
            //var myproducts = db.Product.OrderBy(x => x.product_name).ToList().ToPagedList(page_no, 8);
            var myproducts = db.Product.Where(x=>x.cate_id == id).OrderBy(x => x.product_name).ToList().ToPagedList(page_no, 8);

            return View(myproducts);
        }
    }
}
