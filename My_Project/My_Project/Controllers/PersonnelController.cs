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
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace My_Project.Controllers
{
    public class PersonnelController : Controller
    {
        private Factory_vtEntities db = new Factory_vtEntities();

        public async Task<ActionResult> Index()
        {
            var personnel = db.Personnel.Include(p => p.Country).Include(p => p.Department).Include(p => p.Type);
            return View(await personnel.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel personnel = await db.Personnel.FindAsync(id);
            if (personnel == null)
            {
                return HttpNotFound();
            }
            return View(personnel);
        }

        public ActionResult Create()
        {
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name");
            ViewBag.depart_id = new SelectList(db.Department, "depart_id", "depart_name");
            ViewBag.type_id = new SelectList(db.Type, "type_id", "type_name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "per_id,per_name,gender,per_birth,per_address,country_id,per_phone,per_email,depart_id,type_id,per_username,per_password")] Personnel personnel)
        {
            if (ModelState.IsValid)
            {
                db.Personnel.Add(personnel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name", personnel.country_id);
            ViewBag.depart_id = new SelectList(db.Department, "depart_id", "depart_name", personnel.depart_id);
            ViewBag.type_id = new SelectList(db.Type, "type_id", "type_name", personnel.type_id);
            return View(personnel);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel personnel = await db.Personnel.FindAsync(id);
            if (personnel == null)
            {
                return HttpNotFound();
            }
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name", personnel.country_id);
            ViewBag.depart_id = new SelectList(db.Department, "depart_id", "depart_name", personnel.depart_id);
            ViewBag.type_id = new SelectList(db.Type, "type_id", "type_name", personnel.type_id);
            return View(personnel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "per_id,per_name,gender,per_birth,per_address,country_id,per_phone,per_email,depart_id,type_id,per_username,per_password")] Personnel personnel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name", personnel.country_id);
            ViewBag.depart_id = new SelectList(db.Department, "depart_id", "depart_name", personnel.depart_id);
            ViewBag.type_id = new SelectList(db.Type, "type_id", "type_name", personnel.type_id);
            return View(personnel);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel personnel = await db.Personnel.FindAsync(id);
            if (personnel == null)
            {
                return HttpNotFound();
            }
            return View(personnel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Personnel personnel = await db.Personnel.FindAsync(id);
            db.Personnel.Remove(personnel);
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
            ViewBag.depart_id = new SelectList(db.Department, "depart_id", "depart_name");
            ViewBag.type_id = new SelectList(db.Type, "type_id", "type_name");

            var personnels = db.Personnel.OrderBy(x => x.per_name).ToList();
            return View(personnels);
        }

        [HttpPost]
        public async Task<ActionResult> Listing(int? country_id, int? depart_id, int? type_id)
        {
            ViewBag.country_id = new SelectList(db.Country, "country_id", "country_name");
            ViewBag.depart_id = new SelectList(db.Department, "depart_id", "depart_name");
            ViewBag.type_id = new SelectList(db.Type, "type_id", "type_name");

            IEnumerable<Personnel> mypersonnel;

            if (country_id != null && depart_id != null && type_id != null)//hepsi seçili
                mypersonnel = db.Personnel.Where(x => x.country_id == country_id && x.depart_id == depart_id && x.type_id == type_id).OrderBy(x => x.per_name);
            else if (country_id != null && depart_id != null && type_id == null)//şehir ve bölüm
                mypersonnel = db.Personnel.Where(x => x.country_id == country_id && x.depart_id == depart_id).OrderBy(x => x.per_name);
            else if (country_id != null && depart_id == null && type_id != null)//şehir ve tip
                mypersonnel = db.Personnel.Where(x => x.country_id == country_id && x.type_id == type_id).OrderBy(x => x.per_name);
            else if (country_id == null && depart_id != null && type_id != null)//bölüm ve tip
                mypersonnel = db.Personnel.Where(x => x.depart_id == depart_id && x.type_id == type_id).OrderBy(x => x.per_name);
            else if (country_id != null && depart_id == null && type_id == null)//şehir
                mypersonnel = db.Personnel.Where(x => x.country_id == country_id).OrderBy(x => x.per_name);
            else if (country_id == null && depart_id != null && type_id == null)//bölüm
                mypersonnel = db.Personnel.Where(x => x.depart_id == depart_id).OrderBy(x => x.per_name);
            else if (country_id == null && depart_id == null && type_id != null)//tip
                mypersonnel = db.Personnel.Where(x => x.type_id == type_id).OrderBy(x => x.per_name);
            else//hepsi
                mypersonnel = db.Personnel.ToList();
            var personnels = db.Personnel.OrderBy(x => x.per_name).ToList();
            return View(mypersonnel);
        }
        
        public async Task<ActionResult> Clear()
        {
            return RedirectToAction("Listing");
        }

        public async Task<ActionResult> Crystal_Listing(int? depart_id)
        {
            ViewBag.depart_id = new SelectList(db.Department, "depart_id", "depart_name");

            IEnumerable<Personnel> mypersonnels;

            if(depart_id == null)
            {
                Session["depart"] = null;
                mypersonnels = db.Personnel.OrderBy(x => x.per_name).ToList();
            }
            else
            {
                Session["depart"] = depart_id;
                mypersonnels = db.Personnel.Where(x => x.depart_id == depart_id).OrderBy(x => x.per_name).ToList();
            }
            return View(mypersonnels);
        }

        public async Task<ActionResult> Export(byte? id)
        {
            IQueryable mypers;
            if (Session["depart"] == null)
            {
                mypers = from perr in db.Personnel
                         from count in db.Country
                         from dep in db.Department
                         from typ in db.Type
                         where perr.country_id == count.country_id && perr.depart_id == dep.depart_id && perr.type_id == typ.type_id
                         orderby perr.per_name ascending
                         select new
                         {
                             perr.per_id,
                             perr.per_name,
                             perr.gender,
                             count.country_name,
                             perr.per_phone,
                             dep.depart_name,
                             typ.type_name,
                         };
            }//if
            else
            {
                int depart = Convert.ToInt32(Session["depart"]);
                mypers = from perr in db.Personnel
                         from count in db.Country
                         from dep in db.Department
                         from typ in db.Type
                         where perr.country_id == count.country_id && perr.depart_id == dep.depart_id && perr.type_id == typ.type_id && perr.depart_id == depart
                         orderby perr.per_name ascending
                         select new
                         {
                             perr.per_id,
                             perr.per_name,
                             perr.gender,
                             count.country_name,
                             perr.per_phone,
                             dep.depart_name,
                             typ.type_name,
                         };
            }//else

            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Crystal_report/Per_Report.rpt"));
            report.SetDataSource(mypers);
            Response.Buffer = false;
            Response.ClearContent();

            if(id == 1)
            {
                Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application.pdf", "ahmet.pdf");
            }//if
            else
            {
                Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application.xls", "ahmet.xls");
            }//else
        }
    }
}
